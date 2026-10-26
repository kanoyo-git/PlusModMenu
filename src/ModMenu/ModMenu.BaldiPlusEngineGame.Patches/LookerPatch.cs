using HarmonyLib;
using PlusModMenu.Api;
using PlusModMenu.Api.Characters.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class LookerPatch
{
	[HarmonyPatch(typeof(Looker), "Raycast", new[]
	{
		typeof(Transform),
		typeof(float),
		typeof(PlayerManager),
		typeof(LayerMask),
		typeof(bool)
	}, new[]
	{
		ArgumentType.Normal,
		ArgumentType.Normal,
		ArgumentType.Normal,
		ArgumentType.Normal,
		ArgumentType.Out
	})]
	private class Raycast
	{
		private static bool Prefix(ref bool targetSighted, NPC ___npc)
		{
			if (!(Service.Get<ICharacterHelperService>().GetSpecificCharacterModifiersOfNPC(typeof(ICharacterCanSee), ___npc.GetType()) is ICharacterCanSee { IsBlind: not false }))
			{
				return true;
			}
			targetSighted = false;
			return false;
		}
	}
}
