using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace PlusModMenu.Core.Harmony.Extensions;

public static class CodeInstructionEx
{
	private static bool IsField(CodeInstruction @this, string compilerGeneratedName)
	{
		if (@this.opcode == OpCodes.Ldfld || @this.opcode == OpCodes.Stfld)
		{
			return @this.operand.ToString().EndsWith(compilerGeneratedName, StringComparison.Ordinal);
		}
		return false;
	}

	public static object GetEnumeratorThisField(this IEnumerable<CodeInstruction> @this)
	{
		return @this.FirstOrDefault((CodeInstruction x) => IsField(x, "<>4__this"))?.operand ?? throw new NullReferenceException("The specified collection does not access <>4__this.");
	}

	public static object GetEnumeratorStateField(this IEnumerable<CodeInstruction> @this)
	{
		return @this.FirstOrDefault((CodeInstruction x) => IsField(x, "<>1__state"))?.operand ?? throw new NullReferenceException("The specified collection does not access <>1__state.");
	}

	public static object GetEnumeratorCurrentField(this IEnumerable<CodeInstruction> @this)
	{
		return @this.FirstOrDefault((CodeInstruction x) => IsField(x, "<>2__current"))?.operand ?? throw new NullReferenceException("The specified collection does not access <>2__current.");
	}

	public static bool IsSimple(this CodeInstruction @this, OpCode opCode, object? operand = null)
	{
		if (@this.opcode == opCode)
		{
			return object.Equals(@this.operand, operand);
		}
		return false;
	}
}
