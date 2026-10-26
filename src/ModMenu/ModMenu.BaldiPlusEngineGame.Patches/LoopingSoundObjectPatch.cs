using PlusModMenu.Core.Unity;
using PlusModMenu.Core.Unity.Extensions;
using HarmonyLib;
using PlusModMenu.Api.Collectors;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class LoopingSoundObjectPatch
{
	[HarmonyPatch(typeof(LoopingSoundObject), MethodType.Constructor)]
	private class Constructor
	{
		private static void Postfix(LoopingSoundObject __instance)
		{
			UnityThread.ExecuteCoroutine(__instance.WaitForInitialization(SingletonCollector<LoopingSoundObject>.Add));
		}
	}
}
