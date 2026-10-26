using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Patches.Helpers;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class SubtitleControllerPatch
{
	[HarmonyPatch(typeof(SubtitleController))]
	private class AllMethods : ReplaceCoroutineCalls<SubtitleController>
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			return ReplaceCoroutineCalls<SubtitleController>._TargetMethods();
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return ReplaceCoroutineCalls<SubtitleController>._Transpiler(instructions);
		}
	}
}
