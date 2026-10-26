using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using PlusModMenu.Core.Common.Extensions;
using PlusModMenu.Core.Harmony.Extensions;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.CharacterHelpers;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches.Helpers;

internal class ReplaceCoroutineCalls<T> where T : MonoBehaviour
{
	protected static IEnumerable<MethodBase> _TargetMethods()
	{
		return from x in typeof(T).GetMethods(AccessTools.allDeclared)
			select x.ReturnType.IsAssignableTo(typeof(IEnumerator)) ? AccessTools.EnumeratorMoveNext(x) : x;
	}

	protected static IEnumerable<CodeInstruction> _Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		MethodInfo startCoroutineEnumeratorOriginal = AccessTools.Method(typeof(MonoBehaviour), "StartCoroutine", new Type[1] { typeof(IEnumerator) });
		MethodInfo startCoroutineEnumeratorCustom = AccessTools.Method(typeof(PausableCoroutineHelper), "StartCoroutine", new Type[2]
		{
			typeof(MonoBehaviour),
			typeof(IEnumerator)
		});
		MethodInfo startCoroutineStringOriginal = AccessTools.Method(typeof(MonoBehaviour), "StartCoroutine", new Type[1] { typeof(string) });
		MethodInfo startCoroutineStringCustom = AccessTools.Method(typeof(PausableCoroutineHelper), "StartCoroutine", new Type[2]
		{
			typeof(MonoBehaviour),
			typeof(string)
		});
		MethodInfo stopAllCoroutinesOriginal = AccessTools.Method(typeof(MonoBehaviour), "StopAllCoroutines");
		MethodInfo stopAllCoroutinesCustom = AccessTools.Method(typeof(PausableCoroutineHelper), "StopAllCoroutines", new Type[1] { typeof(MonoBehaviour) });
		foreach (CodeInstruction instruction in instructions)
		{
			if (instruction.IsSimple(OpCodes.Call, startCoroutineEnumeratorOriginal))
			{
				instruction.operand = startCoroutineEnumeratorCustom;
			}
			else if (instruction.IsSimple(OpCodes.Call, startCoroutineStringOriginal))
			{
				instruction.operand = startCoroutineStringCustom;
			}
			else if (instruction.IsSimple(OpCodes.Call, stopAllCoroutinesOriginal))
			{
				instruction.operand = stopAllCoroutinesCustom;
			}
			yield return instruction;
		}
	}
}
