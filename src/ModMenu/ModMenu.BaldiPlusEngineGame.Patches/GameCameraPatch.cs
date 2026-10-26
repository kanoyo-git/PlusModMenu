using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using PlusModMenu.Core.Common;
using PlusModMenu.Core.Harmony.Extensions;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class GameCameraPatch
{
	[HarmonyPatch(typeof(GameCamera), "Awake")]
	private class Awake
	{
		private static void Postfix(GameCamera __instance)
		{
			GameObject gameObject = new GameObject("Camera Rotator");
			gameObject.transform.position = __instance.transform.position;
			gameObject.transform.parent = __instance.camCom.transform.parent;
			GameObject gameObject2 = gameObject;
			__instance.camCom.transform.parent = gameObject2.transform;
			__instance.billboardCam.transform.parent = gameObject2.transform;
		}
	}

	[HarmonyPatch(typeof(GameCamera), "LateUpdate")]
	private class LateUpdate
	{
		private static float CameraFOV => Service.Get<IModMenuService>().Get<PlayerModifiers>().CameraFOV;

		private static float CameraFOVDouble => Math.Min(179f, Service.Get<IModMenuService>().Get<PlayerModifiers>().CameraFOV * 2f);

		private static void Prefix(GameCamera __instance, out Vector3 __state)
		{
			Vector3Patch.IsSafe = true;
			__state = Vector3.zero;
			foreach (CameraModifier cameraModifier in __instance.cameraModifiers)
			{
				__state += cameraModifier.posOffset;
			}
			__instance.offestPos += __state;
		}

		private static void Postfix(GameCamera __instance, Vector3 __state)
		{
			__instance.offestPos -= __state;
			Vector3Patch.IsSafe = false;
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			int patched = 0;
			CodeInstruction[] codeInstructions = instructions.ToArray();
			for (int i = 0; i < codeInstructions.Length; i++)
			{
				CodeInstruction instruction = codeInstructions[i];
				foreach (CodeInstruction item in IfSettingBillboardCamFieldOfViewThenSetModifiedFieldOfViewInstead(i))
				{
					yield return item;
				}
				yield return instruction;
			}
			if (patched != 2)
			{
				Debugger.Warning($"GameCameraPatch.LateUpdate: billboard field of view pattern matched {patched} time(s) instead of 2.");
			}
			IEnumerable<CodeInstruction> IfSettingBillboardCamFieldOfViewThenSetModifiedFieldOfViewInstead(int num)
			{
				if (num + 3 <= codeInstructions.Length - 1)
				{
					FieldInfo operand = AccessTools.Field(typeof(GameCamera), "billboardCam");
					PropertyInfo propertyInfo = AccessTools.Property(typeof(Camera), "fieldOfView");
					PropertyInfo propertyInfo2 = AccessTools.Property(typeof(LateUpdate), "CameraFOV");
					PropertyInfo propertyInfo3 = AccessTools.Property(typeof(LateUpdate), "CameraFOVDouble");
					if (codeInstructions[num].IsSimple(OpCodes.Ldarg_0) && codeInstructions[num + 1].IsSimple(OpCodes.Ldfld, operand) && codeInstructions[num + 2].opcode == OpCodes.Ldc_R4 && codeInstructions[num + 3].IsSimple(OpCodes.Callvirt, propertyInfo.SetMethod) && codeInstructions[num + 2].operand is float num2)
					{
						if (num2 != 120f)
						{
							if (num2 == 60f)
							{
								codeInstructions[num + 2].opcode = OpCodes.Call;
								codeInstructions[num + 2].operand = propertyInfo2.GetMethod;
								patched++;
							}
						}
						else
						{
							codeInstructions[num + 2].opcode = OpCodes.Call;
							codeInstructions[num + 2].operand = propertyInfo3.GetMethod;
							patched++;
						}
					}
				}
				yield break;
			}
		}
	}
}
