using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Patches.Helpers;
using ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;
using ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class BaldiPatch
{
	[HarmonyPatch(typeof(Baldi))]
	private class AllMethods : ReplaceCoroutineCalls<Baldi>
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			return ReplaceCoroutineCalls<Baldi>._TargetMethods();
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return ReplaceCoroutineCalls<Baldi>._Transpiler(instructions);
		}
	}

	[HarmonyPatch(typeof(Baldi_StateBase), "PlayerInSight")]
	private class PlayerInSight
	{
		private static void Prefix(ref bool __state)
		{
			BaldiModifiers baldiModifiers = Service.Get<IModMenuService>().Get<BaldiModifiers>();
			__state = baldiModifiers.IsDeaf;
			baldiModifiers.IsDeaf = false;
		}

		private static void Postfix(ref bool __state)
		{
			Service.Get<IModMenuService>().Get<BaldiModifiers>().IsDeaf = __state;
		}
	}

	[HarmonyPatch(typeof(Baldi), "TakeApple")]
	private class TakeApple
	{
		private static void Prefix(Baldi __instance)
		{
			__instance.appleTime = Service.Get<IModMenuService>().Get<AppleModifiers>().Timer;
		}
	}

	[HarmonyPatch]
	private class OnStateTriggerStay
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.DeclaredMethod(typeof(Baldi_Chase), "OnStateTriggerStay");
			yield return AccessTools.DeclaredMethod(typeof(Baldi_Chase_Tutorial), "OnStateTriggerStay");
		}

		private static bool Prefix(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				return !Service.Get<IModMenuService>().Get<BaldiModifiers>().IgnoresPlayerTrigger;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(Baldi), "Hear")]
	private class Hear
	{
		private static bool Prefix(Baldi __instance)
		{
			return !Service.Get<IModMenuService>().Get<BaldiModifiers>().IsDeaf;
		}
	}

	[HarmonyPatch(typeof(Baldi_Chase), "Update")]
	private class ChaseUpdate
	{
		private static void Prefix(ref float ___delayTimer, NPC ___npc)
		{
			___delayTimer -= Time.deltaTime * ___npc.TimeScale * (Service.Get<IModMenuService>().Get<BaldiModifiers>().SlapSpeed - 1f);
		}
	}

	[HarmonyPatch(typeof(Baldi), "Slap")]
	private class Slap
	{
		private static void Prefix(ref float ___nextSlapDistance)
		{
			___nextSlapDistance *= Service.Get<IModMenuService>().Get<BaldiModifiers>().SlapSpeed;
		}
	}
}
