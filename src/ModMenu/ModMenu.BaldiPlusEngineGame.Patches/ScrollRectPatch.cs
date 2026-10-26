using HarmonyLib;
using UnityEngine.UI;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class ScrollRectPatch
{
	[HarmonyPatch(typeof(ScrollRect), "LateUpdate")]
	private class LateUpdate
	{
		private static void Prefix()
		{
			Vector3Patch.IsSafe = true;
			Vector2Patch.IsSafe = true;
		}

		private static void Postfix()
		{
			Vector3Patch.IsSafe = false;
			Vector2Patch.IsSafe = false;
		}
	}
}
