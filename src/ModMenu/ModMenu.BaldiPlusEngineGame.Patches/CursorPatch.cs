using HarmonyLib;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class CursorPatch
{
	[HarmonyPatch(typeof(Cursor), "lockState", MethodType.Setter)]
	private class LockState
	{
		private static bool Prefix(CursorLockMode value)
		{
			if (ModMenuManager.StateManager.LockStates)
			{
				ModMenuManager.StateManager.CursorLockState = value;
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(Cursor), "visible", MethodType.Setter)]
	private class Visible
	{
		private static bool Prefix(bool value)
		{
			if (ModMenuManager.StateManager.LockStates)
			{
				ModMenuManager.StateManager.CursorVisible = value;
				return false;
			}
			return true;
		}
	}
}
