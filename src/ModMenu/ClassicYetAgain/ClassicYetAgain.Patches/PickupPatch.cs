using ClassicYetAgain.Collectors;
using HarmonyLib;
using PlusModMenu.Api.Collectors;

namespace ClassicYetAgain.Patches;

internal class PickupPatch
{
	[HarmonyPriority(Priority.Low)]
	[HarmonyPatch(typeof(Pickup), "Start")]
	private class Start
	{
		private static void Postfix(Pickup __instance)
		{
			MonoBehaviourCollector<Pickup, ItemCollectorItem>.Add(__instance);
		}
	}
}
