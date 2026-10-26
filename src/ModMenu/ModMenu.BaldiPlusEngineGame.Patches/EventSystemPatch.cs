using HarmonyLib;
using UnityEngine.EventSystems;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class EventSystemPatch
{
	[HarmonyPatch(typeof(EventSystem), "Update")]
	private class Update
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
