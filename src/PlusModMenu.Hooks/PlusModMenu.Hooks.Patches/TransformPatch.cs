using System;
using PlusModMenu.Hooks.Interfaces;
using HarmonyLib;
using UnityEngine;

namespace PlusModMenu.Hooks.Patches;

internal class TransformPatch
{
	[HarmonyPatch(typeof(Transform), "SetParent", new Type[]
	{
		typeof(Transform),
		typeof(bool)
	})]
	private class SetParent
	{
		private static void Prefix(Transform? __instance, ref Transform? __state)
		{
			if (!(__instance == null))
			{
				__state = __instance.parent;
			}
		}

		private static void Postfix(Transform? __instance, ref Transform? __state)
		{
			if (!(__instance == null) && !(__instance.parent == __state))
			{
				ITransformChildRemoved[] array = __state?.GetComponentsInChildren<ITransformChildRemoved>() ?? Array.Empty<ITransformChildRemoved>();
				for (int i = 0; i < array.Length; i++)
				{
					array[i].OnTransformChildRemoved();
				}
				ITransformChildAdded[] array2 = __instance.parent?.GetComponentsInChildren<ITransformChildAdded>() ?? Array.Empty<ITransformChildAdded>();
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].OnTransformChildAdded();
				}
			}
		}
	}
}
