using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using PlusModMenu.Core.Common;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class ITM_PortalPosterPatch
{
	[HarmonyPatch(typeof(ITM_PortalPoster), "Use")]
	private class Use
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo wallHardCovered = AccessTools.Method(typeof(Cell), "WallHardCovered", new System.Type[1] { typeof(Direction) });
			MethodInfo replacement = new System.Func<Cell, Direction, bool>(WallHardCovered).Method;
			bool found = false;
			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.Calls(wallHardCovered))
				{
					found = true;
					yield return new CodeInstruction(OpCodes.Call, replacement).MoveLabelsFrom(instruction).MoveBlocksFrom(instruction);
				}
				else
				{
					yield return instruction;
				}
			}
			if (!found)
			{
				Debugger.Warning("ITM_PortalPosterPatch.Use: Cell.WallHardCovered call not found, \"allow on any wall\" won't work.");
			}
		}

		private static bool WallHardCovered(Cell cell, Direction dir)
		{
			if (AllowOnAnyWall)
			{
				return false;
			}
			return cell.WallHardCovered(dir);
		}
	}

	public static bool AllowOnAnyWall => Service.Get<IModMenuService>().Get<PortalPosterModifiers>().AllowOnAnyWall;
}
