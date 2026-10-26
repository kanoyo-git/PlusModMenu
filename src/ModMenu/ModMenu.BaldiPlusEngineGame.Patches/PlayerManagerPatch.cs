using System;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class PlayerManagerPatch
{
	[HarmonyPatch(typeof(PlayerManager), "RuleBreak", new Type[] { typeof(string), typeof(float), typeof(float) })]
	private class RuleBreak
	{
		private static bool Prefix(PlayerManager __instance, string rule)
		{
			bool num = !Service.Get<IModMenuService>().Get<PrincipalModifiers>().Rules.TryGetValue(rule, out var value) | value;
			if (!num && string.Equals(rule, __instance.ruleBreak, StringComparison.Ordinal))
			{
				__instance.ClearGuilt();
			}
			return num;
		}
	}
}
