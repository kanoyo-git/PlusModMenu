using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using PlusModMenu.Core.Common;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class PartyEventPatch
{
	[HarmonyPatch(typeof(PartyEvent), "Initialize")]
	private class Initialize
	{
		private static void Prefix(PartyEvent __instance)
		{
			PartyEventModifiers partyEventModifiers = Service.Get<IModMenuService>().Get<PartyEventModifiers>();
			__instance.balloonCount = partyEventModifiers.BalloonCount;
		}
	}

	[HarmonyPatch(typeof(PartyEvent), "Begin")]
	private class Begin
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo instantiateBalloon = FindGenericInstantiate();
			MethodInfo replacement = new System.Func<Balloon, Balloon>(InstantiateBalloon).Method;
			bool found = false;
			foreach (CodeInstruction instruction in instructions)
			{
				if (instantiateBalloon != null && instruction.Calls(instantiateBalloon))
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
				Debugger.Warning("PartyEventPatch.Begin: Instantiate<Balloon> call not found, party balloons won't be tracked.");
			}
		}

		private static MethodInfo FindGenericInstantiate()
		{
			foreach (MethodInfo method in typeof(Object).GetMethods(BindingFlags.Static | BindingFlags.Public))
			{
				if (method.Name == "Instantiate" && method.IsGenericMethodDefinition && method.GetParameters().Length == 1)
				{
					return method.MakeGenericMethod(typeof(Balloon));
				}
			}
			return null;
		}

		private static Balloon InstantiateBalloon(Balloon original)
		{
			Balloon balloon = Object.Instantiate(original);
			balloon.gameObject.AddComponent<PartyBalloonIdentifier>();
			return balloon;
		}
	}

	[HarmonyPatch(typeof(PartyEvent), "End")]
	private class End
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo getGameObject = AccessTools.PropertyGetter(typeof(Component), "gameObject");
			MethodInfo destroy = AccessTools.Method(typeof(Object), "Destroy", new System.Type[1] { typeof(Object) });
			MethodInfo safeDestroy = new System.Action<Component>(DestroyGameObjectIfExists).Method;
			List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);
			bool found = false;
			for (int i = 0; i < codeInstructions.Count - 1; i++)
			{
				if (codeInstructions[i].Calls(getGameObject) && codeInstructions[i + 1].Calls(destroy))
				{
					found = true;
					CodeInstruction call = new CodeInstruction(OpCodes.Call, safeDestroy).MoveLabelsFrom(codeInstructions[i]).MoveBlocksFrom(codeInstructions[i]);
					call.labels.AddRange(codeInstructions[i + 1].ExtractLabels());
					codeInstructions[i] = call;
					codeInstructions.RemoveAt(i + 1);
				}
			}
			if (!found)
			{
				Debugger.Warning("PartyEventPatch.End: Destroy(gameObject) calls not found, null checks were not added.");
			}
			return codeInstructions;
		}

		private static void DestroyGameObjectIfExists(Component component)
		{
			if (component != null)
			{
				Object.Destroy(component.gameObject);
			}
		}
	}

	internal class PartyBalloonIdentifier : MonoBehaviour
	{
	}
}
