using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.CharacterHelpers;
using ModMenu.BaldiPlusEngineGame.Patches.Helpers;
using ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class PlaytimePatch
{
	[HarmonyPatch(typeof(Playtime))]
	private class AllMethods : ReplaceCoroutineCalls<Playtime>
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			return ReplaceCoroutineCalls<Playtime>._TargetMethods();
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return ReplaceCoroutineCalls<Playtime>._Transpiler(instructions);
		}
	}

	[HarmonyPatch(typeof(Playtime), "Initialize")]
	private class Initialize
	{
		private static void Postfix(Playtime __instance)
		{
			__instance.gameObject.AddComponent<PlaytimeHelper>();
		}
	}

	[HarmonyPatch(typeof(Playtime), "Count")]
	private class Count
	{
		private static bool Prefix(int jumps)
		{
			return jumps <= 10;
		}
	}

	[HarmonyPatch(typeof(Playtime), "EndJumprope")]
	private class EndJumprope
	{
		private static void Prefix(ref float ___initialCooldown)
		{
			___initialCooldown = Service.Get<IModMenuService>().Get<PlaytimeModifiers>().Cooldown;
		}
	}

	[HarmonyPatch(typeof(Playtime_Wandering), "OnStateTriggerEnter")]
	private class OnStateTriggerEnter
	{
		private static bool Prefix()
		{
			return !Service.Get<IModMenuService>().Get<PlaytimeModifiers>().IgnoresPlayerTrigger;
		}
	}
}
