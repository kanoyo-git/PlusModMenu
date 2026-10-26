using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Patches.Helpers;
using ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class JumpropePatch
{
	[HarmonyPatch(typeof(Jumprope))]
	private class AllMethods : ReplaceCoroutineCalls<Jumprope>
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			return ReplaceCoroutineCalls<Jumprope>._TargetMethods();
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return ReplaceCoroutineCalls<Jumprope>._Transpiler(instructions);
		}
	}

	[HarmonyPatch(typeof(Jumprope), "Start")]
	private class Start
	{
		private static void Prefix(ref int ___maxJumps, ref float ___ropeDelay, ref float ___ropeTime, ref float ___initVelocity, ref float ___accel, Animator ___animator)
		{
			PlaytimeModifiers playtimeModifiers = Service.Get<IModMenuService>().Get<PlaytimeModifiers>();
			if (playtimeModifiers.OverrideJumpropeValues)
			{
				___maxJumps = playtimeModifiers.Jumps;
				___ropeDelay /= playtimeModifiers.JumpropeSpeedMultiplier;
				___ropeTime /= playtimeModifiers.JumpropeSpeedMultiplier;
				___animator.speed = playtimeModifiers.JumpropeSpeedMultiplier;
				___initVelocity *= playtimeModifiers.JumpHeightMultiplier;
				___accel *= playtimeModifiers.JumpHeightMultiplier;
			}
		}
	}
}
