using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using PlusModMenu.Core.Common.Extensions;
using HarmonyLib;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class AchievementManagerPatch
{
	[HarmonyPatch]
	private class AllMethods
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			return from x in typeof(AchievementManager).GetMethods(AccessTools.allDeclared)
				select x.ReturnType.IsAssignableTo(typeof(IEnumerator)) ? AccessTools.EnumeratorMoveNext(x) : x;
		}

		private static IEnumerable<CodeInstruction> Transpiler(MethodBase original)
		{
			Type type = original.ReturnType();
			if (type.IsValueType)
			{
				if (type != typeof(void))
				{
					yield return new CodeInstruction(OpCodes.Ldc_I4_0);
				}
			}
			else
			{
				yield return new CodeInstruction(OpCodes.Ldnull);
			}
			yield return new CodeInstruction(OpCodes.Ret);
		}
	}
}
