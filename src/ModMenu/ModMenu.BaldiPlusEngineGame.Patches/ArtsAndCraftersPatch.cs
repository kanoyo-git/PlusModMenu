using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.CharacterHelpers;
using ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class ArtsAndCraftersPatch
{
	[HarmonyPatch(typeof(ArtsAndCrafters), "Initialize")]
	private class Start
	{
		private static void Postfix(ArtsAndCrafters __instance)
		{
			__instance.gameObject.AddComponent<ArtsAndCraftersHelper>();
		}
	}

	[HarmonyPatch(typeof(ArtsAndCrafters_Chasing), "OnStateTriggerEnter")]
	private class OnTriggerEnter
	{
		private static bool Prefix(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				return !Service.Get<IModMenuService>().Get<CraftersModifiers>().IgnoresPlayerTrigger;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(ArtsAndCrafters), "DisappearForever")]
	private class Attack
	{
		private static void Postfix(ArtsAndCrafters __instance)
		{
			ArtsAndCraftersHelper component = __instance.GetComponent<ArtsAndCraftersHelper>();
			if (component != null)
			{
				component.Reset();
			}
		}
	}

	[HarmonyPatch(typeof(ArtsAndCrafters), "SpawnAt")]
	private class SpawnAt
	{
		private static bool Prefix(ArtsAndCrafters __instance)
		{
			return __instance.gameObject.activeSelf;
		}
	}
}
