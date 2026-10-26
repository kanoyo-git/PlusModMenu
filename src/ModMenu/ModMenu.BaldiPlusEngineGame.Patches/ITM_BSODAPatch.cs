using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class ITM_BSODAPatch
{
	[HarmonyPatch(typeof(ITM_BSODA), "Use")]
	private class Use
	{
		private static void Prefix(ref float ___speed, ref float ___time)
		{
			BSODAModifiers bSODAModifiers = Service.Get<IModMenuService>().Get<BSODAModifiers>();
			___speed = bSODAModifiers.Velocity;
			___time = bSODAModifiers.LifeSpan;
		}

		private static void Postfix(ITM_BSODA __instance)
		{
			if (Camera.main != null)
			{
				__instance.transform.rotation = Camera.main.transform.rotation;
			}
		}
	}
}
