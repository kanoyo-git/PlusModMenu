using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using PlusModMenu.Core.Common;
using HarmonyLib;
using PlusModMenu.Api;
using PlusModMenu.Api.Characters.Interfaces;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class NavigatorPatch
{
	[HarmonyPatch(typeof(Navigator), "MoveTowardsDestination")]
	private class Update
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			FieldInfo distanceToTravel = AccessTools.Field(typeof(Navigator), "_distanceToTravel");
			FieldInfo npc = AccessTools.Field(typeof(Navigator), "npc");
			MethodInfo multiplyVelocity = AccessTools.Method(typeof(Update), "MultiplyVelocity");
			bool firstInstance = false;
			foreach (CodeInstruction instruction in instructions)
			{
				yield return instruction;
				if (!firstInstance && instruction.StoresField(distanceToTravel))
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldflda, distanceToTravel);
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Ldfld, npc);
					yield return new CodeInstruction(OpCodes.Call, multiplyVelocity);
					firstInstance = true;
				}
			}
			if (!firstInstance)
			{
				Debugger.Warning("NavigatorPatch: store of Navigator._distanceToTravel not found, character speed modifiers won't work.");
			}
		}

		private static void MultiplyVelocity(ref float distanceToTravel, NPC npc)
		{
			if (npc != null && Service.Get<ICharacterHelperService>().GetSpecificCharacterModifiersOfNPC(typeof(ICharacterCanMove), npc.GetType()) is ICharacterCanMove characterCanMove)
			{
				distanceToTravel *= characterCanMove.Speed;
			}
		}
	}
}
