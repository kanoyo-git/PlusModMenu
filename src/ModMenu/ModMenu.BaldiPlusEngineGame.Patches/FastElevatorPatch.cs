using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables.OtherModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal static class FastElevatorPatch
{
	public const float Speed = 8f;

	private static readonly AccessTools.FieldRef<ElevatorScreen, BigScreen> _bigScreen = AccessTools.FieldRefAccess<ElevatorScreen, BigScreen>("bigScreen");

	private static readonly AccessTools.FieldRef<ElevatorScreen, Animator> _animator = AccessTools.FieldRefAccess<ElevatorScreen, Animator>("animator");

	private static readonly AccessTools.FieldRef<ElevatorScreen, Animator> _buttonAnimator = AccessTools.FieldRefAccess<ElevatorScreen, Animator>("buttonAnimator");

	internal static int Shown { get; private set; }

	private static bool On => Service.Get<IModMenuService>().Get<WorldModifiers>().FastElevator;

	private static float Delta() => Time.unscaledDeltaTime * (On ? Speed : 1f);

	private static float FixedDelta() => Time.fixedUnscaledDeltaTime * (On ? Speed : 1f);

	[HarmonyPatch]
	private static class Coroutines
	{
		private static readonly MethodInfo _unscaled = AccessTools.PropertyGetter(typeof(Time), nameof(Time.unscaledDeltaTime));

		private static readonly MethodInfo _fixedUnscaled = AccessTools.PropertyGetter(typeof(Time), nameof(Time.fixedUnscaledDeltaTime));

		private static IEnumerable<MethodBase> TargetMethods() =>
			new[] { "ZoomIntro", "ZoomExit", "Results", "BusyDelay", "LoadDelay", "StartDelay" }
				.Select(name => (MethodBase)AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(ElevatorScreen), name)));

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			foreach (var instruction in instructions)
			{
				if (instruction.Calls(_unscaled)) yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FastElevatorPatch), nameof(Delta)));
				else if (instruction.Calls(_fixedUnscaled)) yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FastElevatorPatch), nameof(FixedDelta)));
				else yield return instruction;
			}
		}
	}

	[HarmonyPatch(typeof(ElevatorScreen), "Results")]
	private static class Results
	{
		private static void Postfix()
		{
			if (On) Shown++;
		}
	}

	[HarmonyPatch(typeof(ElevatorScreen), "Update")]
	private static class Animators
	{
		private static void Postfix(ElevatorScreen __instance)
		{
			var speed = On ? Speed : 1f;
			var board = _bigScreen(__instance);
			foreach (var animator in new[] { _animator(__instance), _buttonAnimator(__instance), board != null ? board.animator : null })
			{
				if (animator != null) animator.speed = speed;
			}
		}
	}
}
