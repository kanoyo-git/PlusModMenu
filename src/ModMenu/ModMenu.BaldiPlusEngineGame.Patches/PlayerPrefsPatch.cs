using System;
using HarmonyLib;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class PlayerPrefsPatch
{
	[HarmonyPatch(typeof(PlayerPrefs), "SetString")]
	private class SetString
	{
		private static void Prefix(string key, ref string value)
		{
			if (string.Equals(key, "lastFile", StringComparison.Ordinal) && value.EndsWith(PlayerFileManagerPatch.Suffix, StringComparison.Ordinal))
			{
				string text = value;
				int length = PlayerFileManagerPatch.Suffix.Length;
				value = text.Substring(0, text.Length - length);
			}
		}
	}
}
