using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ClassicYetAgain.Interfaces;
using PlusModMenu.Core.Common;
using PlusModMenu.Core.Common.Utils;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Services;
using PlusModMenu.Api;
using UnityEngine;

namespace ClassicYetAgain.Patches;

internal class EnvironmentControllerPatch
{
	[HarmonyPriority(Priority.Low)]
	[HarmonyPatch(typeof(EnvironmentController), "NPCSpawner", MethodType.Enumerator)]
	private class NPCSpawner
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			if (AssemblyUtils.IsAssemblyDebugBuild(typeof(Baldi).Assembly))
			{
				foreach (CodeInstruction instruction2 in instructions)
				{
					yield return instruction2;
				}
				yield break;
			}
			MethodInfo environmentControllerSpawnNpc = AccessTools.Method(typeof(EnvironmentController), "SpawnNPC");
			MethodInfo npcSpawnerSpawnNpc = AccessTools.Method(typeof(NPCSpawner), "SpawnNPC");
			bool patched = false;
			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.Calls(environmentControllerSpawnNpc))
				{
					instruction.opcode = OpCodes.Call;
					instruction.operand = npcSpawnerSpawnNpc;
					patched = true;
				}
				yield return instruction;
			}
			if (!patched)
			{
				Debugger.Warning("EnvironmentControllerPatch.NPCSpawner: SpawnNPC call not found, \"Activate in Spoop Mode\" won't work.");
			}
		}

		private static NPC SpawnNPC(EnvironmentController ec, NPC npc, IntVector2 position)
		{
			NPC spawnedNpc = ec.SpawnNPC(npc, position);
			SpoopModeCheck(spawnedNpc);
			return spawnedNpc;
		}

		private static void SpoopModeCheck(NPC spawnedNpc)
		{
			if (spawnedNpc == null || !(Service.Get<CharacterHelperService>().GetSpecificCharacterModifiersOfNPC(typeof(ICharacterGetsSpawnedInSpoopMode), spawnedNpc.GetType()) is ICharacterGetsSpawnedInSpoopMode { ActivateInSpoopMode: false }))
			{
				return;
			}
			foreach (SubtitleController item in from x in Resources.FindObjectsOfTypeAll<SubtitleController>()
				where x.soundTran == spawnedNpc.transform
				select x)
			{
				item.gameObject.SetActive(value: false);
			}
			if (spawnedNpc.Entity != null)
			{
				spawnedNpc.Entity.SetActive(value: false);
			}
			else
			{
				spawnedNpc.gameObject.SetActive(value: false);
			}
		}
	}
}
