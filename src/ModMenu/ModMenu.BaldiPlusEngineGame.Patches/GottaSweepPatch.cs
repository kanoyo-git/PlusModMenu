using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.CharacterHelpers;
using ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class GottaSweepPatch
{
	[HarmonyPatch(typeof(GottaSweep), "Start")]
	private class Start
	{
		private static void Postfix(GottaSweep __instance, MovementModifier ___moveMod)
		{
			__instance.gameObject.AddComponent<GottaSweepHelper>();
			___moveMod.movementMultiplier = Service.Get<IModMenuService>().Get<GottaSweepModifiers>().SweepMovementMultiplier;
		}
	}

	[HarmonyPatch(typeof(GottaSweep), "VirtualUpdate")]
	private class Update
	{
		private static void Postfix(MovementModifier ___moveMod)
		{
			___moveMod.movementAddend *= Service.Get<IModMenuService>().Get<GottaSweepModifiers>().SweepPowerMultiplier;
		}
	}

	[HarmonyPatch(typeof(GottaSweep), "VirtualOnTriggerEnter")]
	private class OnTriggerEnter
	{
		private static bool Prefix(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				return !Service.Get<IModMenuService>().Get<GottaSweepModifiers>().IgnoresPlayerTrigger;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(GottaSweep), "GetRandomDelay", MethodType.Getter)]
	private class DelayTimer
	{
		private static void Prefix(ref float ___minDelay, ref float ___maxDelay)
		{
			GottaSweepModifiers gottaSweepModifiers = Service.Get<IModMenuService>().Get<GottaSweepModifiers>();
			___minDelay = gottaSweepModifiers.MinCooldown;
			___maxDelay = gottaSweepModifiers.MaxCooldown;
		}
	}

	[HarmonyPatch(typeof(GottaSweep), "GetRandomSweepTime", MethodType.Getter)]
	private class ActiveTimer
	{
		private static void Prefix(ref float ___minActive, ref float ___maxActive)
		{
			GottaSweepModifiers gottaSweepModifiers = Service.Get<IModMenuService>().Get<GottaSweepModifiers>();
			___minActive = gottaSweepModifiers.MinActive;
			___maxActive = gottaSweepModifiers.MaxActive;
		}
	}
}
