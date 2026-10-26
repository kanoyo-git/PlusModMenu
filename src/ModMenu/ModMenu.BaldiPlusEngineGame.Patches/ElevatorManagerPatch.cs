using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Collectors;
using PlusModMenu.Api.Collectors;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class ElevatorManagerPatch
{
	[HarmonyPatch(typeof(ElevatorManager), "Add")]
	private class Add
	{
		private static void Postfix(Elevator elevator)
		{
			MonoBehaviourCollector<Elevator, ElevatorCollectorItem>.Add(elevator);
		}
	}
}
