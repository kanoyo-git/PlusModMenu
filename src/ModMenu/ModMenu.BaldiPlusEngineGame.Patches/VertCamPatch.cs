using HarmonyLib;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class VertCamPatch
{
	[HarmonyPatch(typeof(VertCam), "Awake")]
	private class Awake
	{
		private static bool Prefix(VertCam __instance)
		{
			__instance.enabled = false;
			return false;
		}
	}
}
