using HarmonyLib;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class TimePatch
{
	[HarmonyPatch(typeof(Time), "timeScale", MethodType.Setter)]
	private class Timescale
	{
		private static bool Prefix(float value)
		{
			if (ModMenuManager.StateManager.LockStates)
			{
				ModMenuManager.StateManager.TimeScale = value;
				return false;
			}
			return true;
		}
	}
}
