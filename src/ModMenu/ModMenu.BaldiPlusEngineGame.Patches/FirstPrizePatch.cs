using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.CharacterHelpers;
using ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;
using ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class FirstPrizePatch
{
	[HarmonyPatch(typeof(FirstPrize), "Initialize")]
	private class Constructor
	{
		private static void Postfix(FirstPrize __instance)
		{
			__instance.gameObject.AddComponent<FirstPrizeHelper>();
		}
	}

	[HarmonyPatch(typeof(FirstPrize), "CutWires")]
	private class CutWires
	{
		private static void Prefix(ref float ___cutTime)
		{
			___cutTime = Service.Get<IModMenuService>().Get<SafetyScissorsModifiers>().FirstPrizeCrazyTime;
		}
	}

	[HarmonyPatch(typeof(FirstPrize_Active), "OnStateTriggerEnter")]
	private class OnTriggerEnter
	{
		private static bool Prefix()
		{
			return !Service.Get<IModMenuService>().Get<FirstPrizeModifiers>().IgnoresPlayerTrigger;
		}
	}

	[HarmonyPatch]
	private class Update
	{
		private static float TurnSpeedMultiplier => Service.Get<IModMenuService>().Get<FirstPrizeModifiers>().RotationSpeedMultiplier;

		private static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(typeof(FirstPrize_Active), "Update");
			yield return AccessTools.Method(typeof(FirstPrize_Stunned), "Update");
		}

		private static void Prefix(NpcState __instance, out float __state)
		{
			FirstPrize firstPrize = (FirstPrize)__instance.Npc;
			__state = firstPrize.turnSpeed;
			firstPrize.turnSpeed *= TurnSpeedMultiplier;
		}

		private static void Postfix(NpcState __instance, float __state)
		{
			((FirstPrize)__instance.Npc).turnSpeed = __state;
		}
	}
}
