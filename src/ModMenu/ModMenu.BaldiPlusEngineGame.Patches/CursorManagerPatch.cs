using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Clipboard;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class CursorManagerPatch
{
	[HarmonyPatch(typeof(CursorManager), "LockCursor")]
	private class LockCursor
	{
		private static bool Prefix()
		{
			return !ClipboardMenu.BlocksGame;
		}
	}
}
