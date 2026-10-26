using System;
using System.IO;
using HarmonyLib;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class PlayerFileManagerPatch
{
	private enum LoadState
	{
		None,
		CopyFromOriginal,
		CreateNew
	}

	[HarmonyPatch(typeof(PlayerFileManager), "Load")]
	private class Load
	{
		private static void Prefix(PlayerFileManager __instance, out LoadState __state)
		{
			string originalName = GetOriginalName(__instance.fileName);
			string moddedName = originalName + Suffix;
			if (File.Exists(GetSavePath(moddedName)))
			{
				__instance.fileName = moddedName;
				__state = LoadState.None;
			}
			else if (File.Exists(GetSavePath(originalName)))
			{
				__instance.fileName = originalName;
				__state = LoadState.CopyFromOriginal;
			}
			else
			{
				__instance.fileName = moddedName;
				__state = LoadState.CreateNew;
			}
		}

		private static void Postfix(PlayerFileManager __instance, ref bool __result, LoadState __state)
		{
			switch (__state)
			{
			case LoadState.CopyFromOriginal:
				__instance.fileName = GetOriginalName(__instance.fileName) + Suffix;
				if (__result)
				{
					__instance.Save();
				}
				break;
			case LoadState.CreateNew:
				__result = true;
				break;
			}
		}
	}

	[HarmonyPatch(typeof(PlayerFileManager), "Delete")]
	private class Delete
	{
		private static void Postfix(PlayerFileManager __instance, string deleteName)
		{
			if (!deleteName.EndsWith(Suffix, StringComparison.Ordinal))
			{
				__instance.Delete(deleteName + Suffix);
			}
		}
	}

	public static string Suffix => "__ModMenu";

	public static string GetOriginalName(string fileName)
	{
		if (!fileName.EndsWith(Suffix, StringComparison.Ordinal))
		{
			return fileName;
		}
		return fileName.Substring(0, fileName.Length - Suffix.Length);
	}

	private static string GetSavePath(string fileName)
	{
		return Application.persistentDataPath + "/PlayerFile_" + fileName + ".sav";
	}
}
