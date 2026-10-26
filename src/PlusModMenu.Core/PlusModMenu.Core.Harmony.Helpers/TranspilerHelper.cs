using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using PlusModMenu.Core.Harmony.Extensions;
using HarmonyLib;

namespace PlusModMenu.Core.Harmony.Helpers;

public class TranspilerHelper
{
	public static IEnumerable<CodeInstruction> EnumerableAffixes(IEnumerable<CodeInstruction> instructions, IEnumerable<CodeInstruction>? prefix = null, IEnumerable<CodeInstruction>? postfix = null)
	{
		if (prefix == null)
		{
			prefix = Enumerable.Empty<CodeInstruction>();
		}
		if (postfix == null)
		{
			postfix = Enumerable.Empty<CodeInstruction>();
		}
		CodeInstruction[] codeInstructions = instructions.ToArray();
		object stateField = codeInstructions.GetEnumeratorStateField();
		bool initialFound = false;
		for (int i = 0; i < codeInstructions.Length; i++)
		{
			if (i == codeInstructions.Length - 2)
			{
				foreach (CodeInstruction item in postfix)
				{
					yield return item;
				}
			}
			yield return codeInstructions[i];
			foreach (CodeInstruction item2 in IfStoringStateM1ForTheFirstTimeThenRunPrefixAfter(i))
			{
				yield return item2;
			}
		}
		IEnumerable<CodeInstruction> IfStoringStateM1ForTheFirstTimeThenRunPrefixAfter(int num)
		{
			if (!((num - 2 < 0) | initialFound) && codeInstructions[num - 2].IsSimple(OpCodes.Ldarg_0) && codeInstructions[num - 1].IsSimple(OpCodes.Ldc_I4_M1) && codeInstructions[num].IsSimple(OpCodes.Stfld, stateField))
			{
				initialFound = true;
				foreach (CodeInstruction item3 in prefix)
				{
					yield return item3;
				}
			}
		}
	}
}
