using System.Collections;
using System.Reflection;
using BepInEx.Bootstrap;
using PlusModMenu.Core.Common.Extensions;
using HarmonyLib;

namespace ModMenu.BepInEx;

public static class BepInUtils
{
	public static IEnumerable WaitForChainloader()
	{
		FieldInfo loaded = AccessTools.Field(typeof(Chainloader), "_loaded");
		while (!loaded.GetValue<bool>(null))
		{
			yield return null;
		}
	}
}
