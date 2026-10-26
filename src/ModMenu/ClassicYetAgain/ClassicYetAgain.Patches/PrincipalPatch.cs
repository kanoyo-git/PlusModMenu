using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ClassicYetAgain.Variables.CharacterModifiers;
using PlusModMenu.Core.Common;
using PlusModMenu.Core.Harmony.Extensions;
using HarmonyLib;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ClassicYetAgain.Patches;

internal class PrincipalPatch
{
	[HarmonyPriority(Priority.Low)]
	[HarmonyPatch(typeof(Principal), "Initialize")]
	private class Start
	{
		private static void Postfix(Principal __instance)
		{
			foreach (Principal item in _detentionCounter.Keys.Where((Principal x) => x == null).ToArray())
			{
				_detentionCounter.Remove(item);
			}
			_detentionCounter[__instance] = 0;
		}
	}

	[HarmonyPriority(Priority.Low)]
	[HarmonyPatch(typeof(Principal), "SendToDetention")]
	private class SendToDetention
	{
		private static bool CustomDetentionEnabled => Service.Get<IModMenuService>().Get<PrincipalModifiers>().CustomDetentionTimesEnabled;

		private static void Postfix(Principal __instance)
		{
			_detentionCounter.TryGetValue(__instance, out var value);
			_detentionCounter[__instance] = value + 1;
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator)
		{
			MethodInfo getCorrectTime = AccessTools.Method(typeof(SendToDetention), "GetCorrectTime");
			MethodInfo customDetentionEnabled = AccessTools.PropertyGetter(typeof(SendToDetention), "CustomDetentionEnabled");
			FieldInfo audMan = AccessTools.Field(typeof(Principal), "audMan");
			FieldInfo audTimes = AccessTools.Field(typeof(Principal), "audTimes");
			FieldInfo detentionLevel = AccessTools.Field(typeof(Principal), "detentionLevel");
			MethodInfo queueAudio = AccessTools.Method(typeof(AudioManager), "QueueAudio", new Type[1] { typeof(SoundObject) });
			Label? branchLabel = null;
			bool audioSkipped = false;
			int timesReplaced = 0;
			CodeInstruction[] codeInstructions = instructions.ToArray();
			for (int i = 0; i < codeInstructions.Length; i++)
			{
				CodeInstruction instruction = codeInstructions[i];
				if (!audioSkipped && i + 7 < codeInstructions.Length && codeInstructions[i].IsSimple(OpCodes.Ldarg_0) && codeInstructions[i + 1].IsSimple(OpCodes.Ldfld, audMan) && codeInstructions[i + 2].IsSimple(OpCodes.Ldarg_0) && codeInstructions[i + 3].IsSimple(OpCodes.Ldfld, audTimes) && codeInstructions[i + 4].IsSimple(OpCodes.Ldarg_0) && codeInstructions[i + 5].IsSimple(OpCodes.Ldfld, detentionLevel) && codeInstructions[i + 6].IsSimple(OpCodes.Ldelem_Ref) && codeInstructions[i + 7].IsSimple(OpCodes.Callvirt, queueAudio))
				{
					branchLabel = ilGenerator.DefineLabel();
					yield return new CodeInstruction(OpCodes.Call, customDetentionEnabled);
					yield return new CodeInstruction(OpCodes.Brtrue_S, branchLabel);
				}
				if (branchLabel.HasValue && i > 0 && codeInstructions[i - 1].IsSimple(OpCodes.Callvirt, queueAudio))
				{
					instruction.labels.Add(branchLabel.Value);
					branchLabel = null;
					audioSkipped = true;
				}
				yield return instruction;
				if (instruction.opcode == OpCodes.Ldloc_1)
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0);
					yield return new CodeInstruction(OpCodes.Call, getCorrectTime);
					timesReplaced++;
				}
			}
			if (!audioSkipped || timesReplaced < 1)
			{
				Debugger.Warning($"PrincipalPatch.SendToDetention: pattern not fully found (audio skip: {audioSkipped}, detention time reads: {timesReplaced}), custom detention times may not work.");
			}
		}

		private static float GetCorrectTime(float time, Principal instance)
		{
			PrincipalModifiers principalModifiers = Service.Get<IModMenuService>().Get<PrincipalModifiers>();
			if (!principalModifiers.CustomDetentionTimesEnabled || principalModifiers.CustomDetentionTimes.Count < 1)
			{
				return time;
			}
			_detentionCounter.TryGetValue(instance, out var value);
			return principalModifiers.CustomDetentionTimes[Math.Min(value, principalModifiers.CustomDetentionTimes.Count - 1)].Time;
		}
	}

	private static readonly Dictionary<Principal, int> _detentionCounter = new Dictionary<Principal, int>();
}
