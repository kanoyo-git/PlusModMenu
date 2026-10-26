using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Clipboard;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class CursorControllerPatch
{
	[HarmonyPatch(typeof(CursorController), "Update")]
	private class Update
	{
		private static bool Prefix()
		{
			return !ClipboardMenu.BlocksGame;
		}
	}
}
