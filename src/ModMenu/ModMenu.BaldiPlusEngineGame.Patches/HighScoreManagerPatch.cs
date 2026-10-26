using HarmonyLib;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class HighScoreManagerPatch
{
	[HarmonyPatch(typeof(HighScoreManager), "AddScore")]
	private class AddScore
	{
		private static bool Prefix(out int rank)
		{
			rank = -1;
			return false;
		}
	}

	[HarmonyPatch(typeof(HighScoreManager), "AddTripScore")]
	private class AddTripScore
	{
		private static bool Prefix(out int rank)
		{
			rank = -1;
			return false;
		}
	}
}
