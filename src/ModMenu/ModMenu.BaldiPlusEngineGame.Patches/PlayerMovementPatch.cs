using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using PlusModMenu.Core.Harmony.Extensions;
using PlusModMenu.Core.Unity;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables;
using PlusModMenu.Api;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class PlayerMovementPatch
{
	[HarmonyPatch(typeof(PlayerMovement), "MouseMove")]
	private class MouseMove
	{
		private static Vector2 _absoluteVector;

		private static Vector2 _deltaVector;

		private static bool Prefix() => !Hl1.CarryController.AnyTurning;

		private static void Postfix(PlayerManager ___pm, AnalogInputData ___cameraAnalogData)
		{
			if (Hl1.CarryController.AnyTurning)
			{
				return;
			}
			PlayerModifiers playerModifiers = Service.Get<IModMenuService>().Get<PlayerModifiers>();
			if (Singleton<CoreGameManager>.Instance == null)
			{
				return;
			}
			GameCamera camera = Singleton<CoreGameManager>.Instance.GetCamera(0);
			if (!(camera == null))
			{
				Transform parent = camera.camCom.transform.parent;
				if (playerModifiers.CameraMode == PlayerModifiers.PlayerCameraMode.Locked || !camera.Controllable || Singleton<PlayerFileManager>.Instance.authenticMode)
				{
					parent.localRotation = ClampRotationAroundXAxis(parent.localRotation, 0f, 0f);
					return;
				}
				Singleton<InputManager>.Instance.GetAnalogInput(___cameraAnalogData, out _absoluteVector, out _deltaVector, 0.1f);
				float num = _deltaVector.y * Singleton<PlayerFileManager>.Instance.mouseCameraSensitivity + _absoluteVector.y * Time.deltaTime * Singleton<PlayerFileManager>.Instance.controllerCameraSensitivity;
				num = num * Time.timeScale * ___pm.PlayerTimeScale;
				Quaternion localRotation = parent.localRotation;
				localRotation *= Quaternion.Euler(0f - num, 0f, 0f);
				localRotation = ClampRotationAroundXAxis(localRotation, -89f, 89f);
				parent.transform.localRotation = localRotation;
			}
		}

		private static Quaternion ClampRotationAroundXAxis(Quaternion q, float min, float max)
		{
			q.x /= q.w;
			q.y /= q.w;
			q.z /= q.w;
			q.w = 1f;
			float value = 114.59156f * Mathf.Atan(q.x);
			value = Mathf.Clamp(value, min, max);
			q.x = Mathf.Tan((float)Math.PI / 360f * value);
			return q;
		}
	}

	[HarmonyPatch(typeof(PlayerMovement), "PlayerMove")]
	private class PlayerMove
	{
		public static bool Noclip => Service.Get<IModMenuService>().Get<PlayerModifiers>().Noclip;

		private static void Prefix(PlayerMovement __instance, out float __state)
		{
			__state = __instance.transform.position.y;
		}

		private static void Postfix(PlayerMovement __instance, float __state, Vector3 ____moveX, Vector2 ____absoluteVector)
		{
			if (Noclip)
			{
				Vector3 position = __instance.transform.position;
				position.y = __state;
				__instance.transform.position = position;
				GameCamera gameCamera = Singleton<CoreGameManager>.Instance?.GetCamera(0);
				if (gameCamera != null && !__instance.Entity.InteractionDisabled)
				{
					Transform parent = gameCamera.camCom.transform.parent;
					Vector3 moveZ = Quaternion.Euler(parent.eulerAngles + new Vector3(0f, __instance.transform.eulerAngles.y - parent.eulerAngles.y, 0f)) * Vector3.forward * ____absoluteVector.y;
					__instance.Entity.UpdateInternalMovement((____moveX + moveZ).normalized * __instance.Entity.InternalMovement.magnitude);
				}
			}
			UpdateSuperHotTimescales(__instance.RealVelocity * __instance.RealVelocity / 100f);
		}

		internal static Vector3 NoclipMovement(Vector3 currentPosition, Vector3 desiredMovement, out float properVelocity)
		{
			properVelocity = 0f;
			if (desiredMovement.IsValid())
			{
				Vector3 vector = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z);
				currentPosition += desiredMovement;
				Vector3 vector2 = currentPosition - vector;
				properVelocity = (vector2 / Time.deltaTime).sqrMagnitude / 100f;
				if (float.IsNaN(properVelocity))
				{
					properVelocity = 0f;
				}
			}
			return currentPosition;
		}

		private static void UpdateSuperHotTimescales(float velocity)
		{
			GameplayModifiers gameplayModifiers = Service.Get<IModMenuService>().Get<GameplayModifiers>();
			if (gameplayModifiers.SuperHot)
			{
				gameplayModifiers.SuperHotTimeScaleModifier.npcTimeScale = Mathf.Clamp01(Mathf.Lerp(gameplayModifiers.SuperHotTimeScaleModifier.npcTimeScale, velocity, 0.2f) + 0.02f);
				gameplayModifiers.SuperHotTimeScaleModifier.environmentTimeScale = Mathf.Clamp01(Mathf.Lerp(gameplayModifiers.SuperHotTimeScaleModifier.environmentTimeScale, velocity, 0.2f) + 0.02f);
			}
			else
			{
				gameplayModifiers.SuperHotTimeScaleModifier.npcTimeScale = 1f;
				gameplayModifiers.SuperHotTimeScaleModifier.environmentTimeScale = 1f;
			}
		}
	}

	[HarmonyPatch(typeof(Entity), "EntityUpdate")]
	private class EntityUpdate
	{
		private static bool Prefix(Entity __instance, bool ___isPlayer, Vector3 ___internalMovementVector, ActivityModifier ___externalActivity, List<Force> ___forces, ref Vector3 ___previousPosition, ref Vector3 ___velocity, ref Vector3 ___relativeToForceVelocity, ref Vector3 ___expectedForcedPosition)
		{
			if (!___isPlayer)
			{
				return true;
			}
			Vector3Patch.IsSafe = true;
			Vector2Patch.IsSafe = true;
			if (!PlayerMove.Noclip || __instance.Ec == null)
			{
				return true;
			}
			Transform transform = __instance.transform;
			if (!__instance.Frozen)
			{
				Vector3 vector = Vector3.zero;
				for (int i = 0; i < ___forces.Count; i++)
				{
					vector += ___forces[i].VelocityThisFrame(Time.deltaTime);
					if (___forces[i].Dead)
					{
						___forces.RemoveAt(i);
						i--;
					}
				}
				Vector3 addend = ___externalActivity.Addend;
				addend.y = 0f;
				vector.y = 0f;
				___expectedForcedPosition = transform.position + (addend * Time.deltaTime + vector);
				transform.position = PlayerMove.NoclipMovement(transform.position, (___internalMovementVector * ___externalActivity.Multiplier + addend) * Time.deltaTime + vector, out var _);
			}
			else
			{
				___expectedForcedPosition = transform.position;
			}
			___velocity = transform.position - ___previousPosition;
			___relativeToForceVelocity = transform.position - ___expectedForcedPosition;
			___previousPosition = transform.position;
			return false;
		}

		private static void Postfix(bool ___isPlayer)
		{
			if (___isPlayer)
			{
				Vector3Patch.IsSafe = false;
				Vector2Patch.IsSafe = false;
			}
		}
	}

	[HarmonyPatch(typeof(PlayerMovement), "StaminaUpdate")]
	private class StaminaUpdate
	{
		private static void Postfix(PlayerMovement __instance)
		{
			if (Service.Get<IModMenuService>().Get<PlayerModifiers>().InfiniteStamina)
			{
				__instance.stamina = __instance.StaminaMax;
			}
		}
	}

	[HarmonyPatch(typeof(PlayerMovement), "Start")]
	private class Start
	{
		private static void Postfix()
		{
			Service.Get<IEventService>().Publish<IPlayerInstantiatedEvent>(null);
		}
	}

	[HarmonyPatch(typeof(PlayerMovement), "Update")]
	private class Update
	{
		private static void Prefix()
		{
			Vector3Patch.IsSafe = true;
			Vector2Patch.IsSafe = true;
		}

		private static void Postfix()
		{
			Vector3Patch.IsSafe = false;
			Vector2Patch.IsSafe = false;
		}
	}
}
