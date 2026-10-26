using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using PlusModMenu.Core.Common;
using PlusModMenu.Core.Harmony.Extensions;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.CharacterHelpers;
using ModMenu.BaldiPlusEngineGame.Patches.Helpers;
using ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using TMPro;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class NoLateTeacherPatch
{
	[HarmonyPatch(typeof(NoLateTeacher))]
	private class AllMethods : ReplaceCoroutineCalls<NoLateTeacher>
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			return ReplaceCoroutineCalls<NoLateTeacher>._TargetMethods();
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return ReplaceCoroutineCalls<NoLateTeacher>._Transpiler(instructions);
		}
	}

	[HarmonyPatch(typeof(NoLateTeacher), "Initialize")]
	private class Initialize
	{
		private static void Postfix(NoLateTeacher __instance, TMP_Text ___popupText)
		{
			__instance.gameObject.AddComponent<MrsPompHelper>();
			___popupText.rectTransform.sizeDelta = new Vector2(300f, 50f);
		}
	}

	[HarmonyPatch(typeof(NoLateTeacher_Wander), "OnStateTriggerStay")]
	private class OnStateTriggerStay
	{
		private static bool Prefix(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				return !Service.Get<IModMenuService>().Get<MrsPompModifiers>().IgnoresPlayerTrigger;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(NoLateTeacher), "PlayerCaught")]
	private class PlayerCaught
	{
		private static void Prefix(ref float ___classTime)
		{
			___classTime = Service.Get<IModMenuService>().Get<MrsPompModifiers>().ClassTimerLength;
		}
	}

	[HarmonyPatch]
	private class AudNumbersIndexCheck
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.DeclaredMethod(typeof(NoLateTeacher), "PlayerCaught");
			yield return AccessTools.DeclaredMethod(typeof(NoLateTeacher), "UpdateTimer");
		}

		private static void QueueNumber(AudioManager audioManager, SoundObject[] audNumbers, int index)
		{
			if (index > -1 && index < audNumbers.Length)
			{
				audioManager.QueueAudio(audNumbers[index]);
			}
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			FieldInfo audNumbers = AccessTools.Field(typeof(NoLateTeacher), "audNumbers");
			MethodInfo queueAudio = AccessTools.Method(typeof(AudioManager), "QueueAudio", new Type[1] { typeof(SoundObject) });
			MethodInfo queueNumber = AccessTools.Method(typeof(AudNumbersIndexCheck), "QueueNumber");
			List<CodeInstruction> codeInstructions = instructions.ToList();
			bool patched = false;
			for (int i = 0; i < codeInstructions.Count; i++)
			{
				if (!codeInstructions[i].IsSimple(OpCodes.Ldfld, audNumbers))
				{
					continue;
				}
				for (int j = i + 1; j + 1 < codeInstructions.Count && j <= i + 8; j++)
				{
					if (codeInstructions[j].IsSimple(OpCodes.Ldelem_Ref) && codeInstructions[j + 1].IsSimple(OpCodes.Callvirt, queueAudio))
					{
						CodeInstruction call = new CodeInstruction(OpCodes.Call, queueNumber);
						call.labels.AddRange(codeInstructions[j].labels);
						call.labels.AddRange(codeInstructions[j + 1].labels);
						codeInstructions[j] = call;
						codeInstructions.RemoveAt(j + 1);
						patched = true;
						break;
					}
				}
			}
			if (!patched)
			{
				Debugger.Warning("NoLateTeacherPatch.AudNumbersIndexCheck: audNumbers pattern not found in " + original.Name + ".");
			}
			return codeInstructions;
		}
	}

	[HarmonyPatch(typeof(NoLateTeacher), "Cooldown", MethodType.Getter)]
	private class Cooldown
	{
		private static void Postfix(ref float __result)
		{
			__result = Service.Get<IModMenuService>().Get<MrsPompModifiers>().ResetDelay;
		}
	}
}
