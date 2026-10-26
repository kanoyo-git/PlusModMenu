using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class UseItemsAnywherePatch
{
	private static bool On => Service.Get<IModMenuService>().Get<InventoryModifiers>().UseItemsAnywhere;

	[HarmonyPatch(typeof(ItemManager), nameof(ItemManager.UseItem))]
	private class UseItem
	{
		private static void Prefix(ref bool ___disabled, out bool __state)
		{
			__state = ___disabled && On;
			if (__state)
			{
				___disabled = false;
			}
		}

		private static void Finalizer(ref bool ___disabled, bool __state)
		{
			if (__state)
			{
				___disabled = true;
			}
		}
	}

	[HarmonyPatch(typeof(ItemManager), "Update")]
	private class OutOfBounds
	{
		private static void Postfix(ItemManager __instance)
		{
			var pm = __instance.pm;
			if (!On || pm == null || Time.timeScale == 0f || !Singleton<BaseGameManager>.Instance.GameReady || pm.plm.Entity.InBounds || pm.plm.Entity.InteractionDisabled
				|| Singleton<PlayerFileManager>.Instance.authenticMode || !Singleton<InputManager>.Instance.GetDigitalInput("UseItem", onDown: true))
			{
				return;
			}
			__instance.UseItem();
		}
	}
}
