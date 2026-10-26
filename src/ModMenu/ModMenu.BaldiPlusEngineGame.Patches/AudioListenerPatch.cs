using HarmonyLib;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class AudioListenerPatch
{
	[HarmonyPatch(typeof(AudioListener), "pause", MethodType.Setter)]
	private class Pause
	{
		private static bool Prefix(bool value)
		{
			if (ModMenuManager.StateManager.LockStates)
			{
				ModMenuManager.StateManager.AudioListenerPause = value;
				return false;
			}
			return true;
		}
	}
}
