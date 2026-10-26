using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using PlusModMenu.Core.Common;
using PlusModMenu.Core.Harmony.Extensions;
using HarmonyLib;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class PlayerClickPatch
{
	[HarmonyPatch(typeof(PlayerClick), "Update")]
	private class Update
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo getCamera = AccessTools.Method(typeof(CoreGameManager), "GetCamera");
			MethodInfo direction = AccessTools.PropertySetter(typeof(Ray), "direction");
			FieldInfo camCom = AccessTools.Field(typeof(GameCamera), "camCom");
			bool patched = false;
			CodeInstruction[] codeInstructions = instructions.ToArray();
			for (int i = 0; i < codeInstructions.Length; i++)
			{
				CodeInstruction instruction = codeInstructions[i];
				yield return instruction;
				if (instruction.IsSimple(OpCodes.Callvirt, getCamera) && i + 3 < codeInstructions.Length && codeInstructions[i + 3].IsSimple(OpCodes.Call, direction))
				{
					yield return new CodeInstruction(OpCodes.Ldfld, camCom);
					patched = true;
				}
			}
			if (!patched)
			{
				Debugger.Warning("PlayerClickPatch.Update: click ray direction pattern not found.");
			}
		}
	}
}
