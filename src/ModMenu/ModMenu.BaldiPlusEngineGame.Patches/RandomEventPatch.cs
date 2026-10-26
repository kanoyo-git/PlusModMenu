using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Collectors;
using PlusModMenu.Api.Collectors;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class RandomEventPatch
{
	[HarmonyPatch(typeof(RandomEvent), "Begin")]
	private class Begin
	{
		private static void Postfix(RandomEvent __instance)
		{
			MonoBehaviourCollector<RandomEvent, EventCollectorItem>.Add(__instance);
		}
	}

	[HarmonyPatch(typeof(RandomEvent), "End")]
	private class End
	{
		private static void Postfix(RandomEvent __instance)
		{
			if (__instance.TryGetComponent<EventCollectorItem>(out var component))
			{
				Object.Destroy(component);
			}
		}
	}
}
