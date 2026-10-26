using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.CharacterHelpers;
using ModMenu.BaldiPlusEngineGame.Patches.Helpers;
using ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class BullyPatch
{
	[HarmonyPatch(typeof(Bully))]
	private class AllMethods : ReplaceCoroutineCalls<Bully>
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			return ReplaceCoroutineCalls<Bully>._TargetMethods();
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return ReplaceCoroutineCalls<Bully>._Transpiler(instructions);
		}
	}

	[HarmonyPatch(typeof(Bully), "Initialize")]
	private class Initialize
	{
		private static void Prefix(Bully __instance)
		{
			SetTimers(__instance);
		}

		private static void Postfix(Bully __instance)
		{
			__instance.gameObject.AddComponent<BullyHelper>();
		}
	}

	[HarmonyPatch(typeof(Bully_Active), "OnStateTriggerEnter")]
	private class OnStateTriggerEnter
	{
		private static bool Prefix(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				return !IgnoresPlayerTrigger;
			}
			return true;
		}
	}

	[HarmonyPatch]
	private class Timers
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.DeclaredMethod(typeof(Bully), "Hide");
			yield return AccessTools.DeclaredMethod(typeof(Bully), "Spawn");
		}

		private static void Prefix(Bully __instance)
		{
			SetTimers(__instance);
		}
	}

	private static readonly FieldInfo _minDelay = AccessTools.Field(typeof(Bully), "minDelay");

	private static readonly FieldInfo _maxDelay = AccessTools.Field(typeof(Bully), "maxDelay");

	private static readonly FieldInfo _maxStay = AccessTools.Field(typeof(Bully), "maxStay");

	private static bool IgnoresPlayerTrigger => Service.Get<IModMenuService>().Get<BullyModifiers>().IgnoresPlayerTrigger;

	private static readonly ConditionalWeakTable<Bully, float[]> _originalTimers = new ConditionalWeakTable<Bully, float[]>();

	private static void SetTimers(Bully bully)
	{
		BullyModifiers bullyModifiers = Service.Get<IModMenuService>().Get<BullyModifiers>();
		float[] original = _originalTimers.GetValue(bully, (Bully x) => new float[3]
		{
			(float)_minDelay.GetValue(x),
			(float)_maxDelay.GetValue(x),
			(float)_maxStay.GetValue(x)
		});
		_minDelay.SetValue(bully, (bullyModifiers.MinCooldown == 60f) ? original[0] : bullyModifiers.MinCooldown);
		_maxDelay.SetValue(bully, (bullyModifiers.MaxCooldown == 120f) ? original[1] : bullyModifiers.MaxCooldown);
		_maxStay.SetValue(bully, (bullyModifiers.MaxHalltime == 120f) ? original[2] : bullyModifiers.MaxHalltime);
	}
}
