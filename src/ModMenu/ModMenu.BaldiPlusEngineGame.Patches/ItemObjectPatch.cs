using PlusModMenu.Core.Unity;
using PlusModMenu.Core.Unity.Extensions;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Collectors;
using PlusModMenu.Api.Collectors;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class ItemObjectPatch
{
	[HarmonyPatch(typeof(ItemObject), MethodType.Constructor)]
	private class Constructor
	{
		private static void Postfix(ItemObject __instance)
		{
			UnityThread.ExecuteCoroutine(__instance.WaitForInitialization(DistinctSingletonCollector<ItemObject, ItemObjectEqualityComparer>.Add));
		}
	}
}
