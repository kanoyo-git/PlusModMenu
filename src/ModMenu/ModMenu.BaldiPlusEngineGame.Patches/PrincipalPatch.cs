using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using PlusModMenu.Core.Common;
using PlusModMenu.Core.Harmony.Extensions;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Patches.Helpers;
using ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class PrincipalPatch
{
	[HarmonyPatch(typeof(Principal))]
	private class AllMethods : ReplaceCoroutineCalls<Principal>
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			return ReplaceCoroutineCalls<Principal>._TargetMethods();
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return ReplaceCoroutineCalls<Principal>._Transpiler(instructions);
		}
	}

	[HarmonyPatch(typeof(Principal_ChasingPlayer), "OnStateTriggerStay")]
	private class OnStateTriggerStay
	{
		private static bool Prefix(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				return !Service.Get<IModMenuService>().Get<PrincipalModifiers>().IgnoresPlayerTrigger;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(Principal), "WhistleReact")]
	private class WhistleReact
	{
		private static bool Prefix()
		{
			return !Service.Get<IModMenuService>().Get<PrincipalModifiers>().IsDeaf;
		}
	}

	[HarmonyPatch(typeof(Principal), "SendToDetention")]
	private class SendToDetention
	{
		private static readonly ConditionalWeakTable<Principal, StrongBox<float>> _originalDetentionInc = new ConditionalWeakTable<Principal, StrongBox<float>>();

		private static void Prefix(Principal __instance, ref float ___detentionInc)
		{
			float current = ___detentionInc;
			float original = _originalDetentionInc.GetValue(__instance, (Principal _) => new StrongBox<float>(current)).Value;
			int detentionTimeIncrease = Service.Get<IModMenuService>().Get<PrincipalModifiers>().DetentionTimeIncrease;
			___detentionInc = ((detentionTimeIncrease == 5) ? original : ((float)detentionTimeIncrease));
		}

		private static bool HasOriginalDetentionInc(Principal principal, float detentionInc)
		{
			if (_originalDetentionInc.TryGetValue(principal, out StrongBox<float> value))
			{
				return value.Value == detentionInc;
			}
			return true;
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator)
		{
			FieldInfo audMan = AccessTools.Field(typeof(Principal), "audMan");
			FieldInfo audTimes = AccessTools.Field(typeof(Principal), "audTimes");
			FieldInfo detentionLevel = AccessTools.Field(typeof(Principal), "detentionLevel");
			FieldInfo detentionInc = AccessTools.Field(typeof(Principal), "detentionInc");
			MethodInfo hasOriginalDetentionInc = AccessTools.Method(typeof(SendToDetention), "HasOriginalDetentionInc");
			MethodInfo queueAudio = AccessTools.Method(typeof(AudioManager), "QueueAudio", new Type[1] { typeof(SoundObject) });
			CodeInstruction[] codeInstructions = instructions.ToArray();
			for (int i = 0; i + 8 < codeInstructions.Length; i++)
			{
				if (codeInstructions[i].IsSimple(OpCodes.Ldarg_0) && codeInstructions[i + 1].IsSimple(OpCodes.Ldfld, audMan) && codeInstructions[i + 2].IsSimple(OpCodes.Ldarg_0) && codeInstructions[i + 3].IsSimple(OpCodes.Ldfld, audTimes) && codeInstructions[i + 4].IsSimple(OpCodes.Ldarg_0) && codeInstructions[i + 5].IsSimple(OpCodes.Ldfld, detentionLevel) && codeInstructions[i + 6].IsSimple(OpCodes.Ldelem_Ref) && codeInstructions[i + 7].IsSimple(OpCodes.Callvirt, queueAudio))
				{
					Label branchLabel = ilGenerator.DefineLabel();
					codeInstructions[i + 8].labels.Add(branchLabel);
					CodeInstruction check = new CodeInstruction(OpCodes.Ldarg_0);
					check.labels.AddRange(codeInstructions[i].labels);
					codeInstructions[i].labels.Clear();
					return codeInstructions.Take(i).Concat(new CodeInstruction[5]
					{
						check,
						new CodeInstruction(OpCodes.Ldarg_0),
						new CodeInstruction(OpCodes.Ldfld, detentionInc),
						new CodeInstruction(OpCodes.Call, hasOriginalDetentionInc),
						new CodeInstruction(OpCodes.Brfalse, branchLabel)
					}).Concat(codeInstructions.Skip(i));
				}
			}
			Debugger.Warning("PrincipalPatch.SendToDetention: detention time audio pattern not found.");
			return codeInstructions;
		}
	}
}
