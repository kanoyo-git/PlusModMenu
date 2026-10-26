using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class GumPatch
{
	[HarmonyPatch(typeof(Gum), "EntityTriggerEnter")]
	private class EntityTriggerEnter
	{
		private static bool Prefix(Collider other, ref float ___setTime, MovementModifier ___playerMod, MovementModifier ___moveMod)
		{
			BeansModifiers beansModifiers = Service.Get<IModMenuService>().Get<BeansModifiers>();
			___setTime = Mathf.Max(beansModifiers.GumStuckTime, float.Epsilon);
			___playerMod.movementMultiplier = beansModifiers.GumMovementMultiplierPlayer;
			___moveMod.movementMultiplier = beansModifiers.GumMovementMultiplierNPC;
			if (beansModifiers.IgnoresPlayerTrigger)
			{
				return !other.CompareTag("Player");
			}
			return true;
		}
	}
}
