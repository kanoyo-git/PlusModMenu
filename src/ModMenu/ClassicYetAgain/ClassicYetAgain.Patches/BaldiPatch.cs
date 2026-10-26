using System.Runtime.CompilerServices;
using ClassicYetAgain.Variables.CharacterModifiers;
using HarmonyLib;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ClassicYetAgain.Patches;

internal class BaldiPatch
{
	[HarmonyPriority(Priority.Low)]
	[HarmonyPatch(typeof(Baldi), "Delay", MethodType.Getter)]
	private class SlapTimer
	{
		private static void Postfix(Baldi __instance, AnimationCurve ___slapCurve, ref float __result)
		{
			if (IsActive(__instance, ___slapCurve))
			{
				__result = _randomDelays.GetValue(__instance, (Baldi _) => new StrongBox<float>(RandomDelay(___slapCurve))).Value;
			}
		}
	}

	[HarmonyPriority(Priority.Low)]
	[HarmonyPatch(typeof(Baldi), "Slap")]
	private class Slap
	{
		private static void Prefix(Baldi __instance, AnimationCurve ___slapCurve)
		{
			if (IsActive(__instance, ___slapCurve))
			{
				_randomDelays.GetOrCreateValue(__instance).Value = RandomDelay(___slapCurve);
			}
		}
	}

	private static readonly ConditionalWeakTable<Baldi, StrongBox<float>> _randomDelays = new ConditionalWeakTable<Baldi, StrongBox<float>>();

	private static bool IsActive(Baldi instance, AnimationCurve slapCurve)
	{
		if (instance.GetType() == typeof(Baldi) && slapCurve != null && slapCurve.length >= 1)
		{
			return Service.Get<IModMenuService>().Get<BaldiModifiers>().SlapSpeedRandomizer;
		}
		return false;
	}

	private static float RandomDelay(AnimationCurve slapCurve)
	{
		return slapCurve.Evaluate(Random.Range(0f, slapCurve[slapCurve.length - 1].time));
	}
}
