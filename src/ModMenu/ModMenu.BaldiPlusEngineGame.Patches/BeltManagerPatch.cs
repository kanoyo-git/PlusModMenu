using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.CharacterHelpers;
using ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class BeltManagerPatch
{
	[HarmonyPatch(typeof(BeltManager), "OnTriggerEnter")]
	private class OnTriggerEnter
	{
		private static bool Prefix(BeltManager __instance, Collider other)
		{
			if (other.CompareTag("Player") && !(__instance.GetComponent<CloudyCopterHelper.WindTriggerIdentifier>() == null))
			{
				return !Service.Get<IModMenuService>().Get<CloudyCopterModifiers>().IgnoresPlayerTrigger;
			}
			return true;
		}
	}
}
