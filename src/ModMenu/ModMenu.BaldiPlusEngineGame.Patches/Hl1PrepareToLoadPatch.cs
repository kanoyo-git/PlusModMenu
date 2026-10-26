using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Hl1;

namespace ModMenu.BaldiPlusEngineGame.Patches;

[HarmonyPatch(typeof(BaseGameManager), "PrepareToLoad")]
internal class Hl1PrepareToLoadPatch
{
	private static void Prefix() => CarryController.DropAll();
}
