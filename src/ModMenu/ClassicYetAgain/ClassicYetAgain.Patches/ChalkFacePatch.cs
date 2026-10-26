using ClassicYetAgain.BaldiClassicRemastered;
using HarmonyLib;

namespace ClassicYetAgain.Patches;

internal class ChalkFacePatch
{
	[HarmonyPriority(Priority.Low)]
	[HarmonyPatch(typeof(ChalkFace), "Cancelled")]
	private class Cancel
	{
		private static void Postfix(ChalkFace __instance)
		{
			SetLethalTouch(__instance, value: false);
		}
	}

	[HarmonyPriority(Priority.Low)]
	[HarmonyPatch(typeof(ChalkFace), "Activate")]
	private class Timer
	{
		private static void Postfix(ChalkFace __instance)
		{
			SetLethalTouch(__instance, value: true);
		}
	}

	private static void SetLethalTouch(ChalkFace chalkFace, bool value)
	{
		LethalTouch componentInChildren = chalkFace.GetComponentInChildren<LethalTouch>(includeInactive: true);
		if (componentInChildren != null)
		{
			componentInChildren.enabled = value;
		}
	}
}
