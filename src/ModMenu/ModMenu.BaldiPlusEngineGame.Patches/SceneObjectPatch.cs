using PlusModMenu.Core.Unity;
using PlusModMenu.Core.Unity.Extensions;
using HarmonyLib;
using PlusModMenu.Api.Collectors;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class SceneObjectPatch
{
	[HarmonyPatch(typeof(SceneObject), MethodType.Constructor)]
	private class Constructor
	{
		private static void Postfix(SceneObject __instance)
		{
			UnityThread.ExecuteCoroutine(__instance.WaitForInitialization(SingletonCollector<SceneObject>.Add));
		}
	}
}
