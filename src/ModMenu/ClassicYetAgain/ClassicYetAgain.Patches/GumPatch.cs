using ClassicYetAgain.BaldiClassicRemastered;
using ClassicYetAgain.Variables.CharacterModifiers;
using HarmonyLib;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ClassicYetAgain.Patches;

internal class GumPatch
{
	[HarmonyPriority(Priority.Low)]
	[HarmonyPatch(typeof(Gum), "EntityTriggerEnter")]
	private class OnTriggerEnter
	{
		private static void Prefix(bool ___flying, out bool __state)
		{
			__state = ___flying;
		}

		private static void Postfix(Collider other, Beans ___beans, bool ___flying, bool __state)
		{
			if (__state && !___flying && other.CompareTag("Player") && Service.Get<IModMenuService>().Get<BeansModifiers>().LethalTouchOnGumWad && ___beans != null)
			{
				LethalTouch componentInChildren = ___beans.GetComponentInChildren<LethalTouch>(includeInactive: true);
				if (componentInChildren != null)
				{
					componentInChildren.ForceEnd(other);
				}
			}
		}
	}
}
