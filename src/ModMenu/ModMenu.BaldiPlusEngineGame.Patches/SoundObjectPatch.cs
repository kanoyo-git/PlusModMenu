using PlusModMenu.Core.Unity;
using PlusModMenu.Core.Unity.Extensions;
using HarmonyLib;
using PlusModMenu.Api.Collectors;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class SoundObjectPatch
{
	[HarmonyPatch(typeof(SoundObject), MethodType.Constructor)]
	private class Constructor
	{
		private static void Postfix(SoundObject __instance)
		{
			UnityThread.ExecuteCoroutine(__instance.WaitForInitialization(SingletonCollector<SoundObject>.Add));
		}
	}
}
