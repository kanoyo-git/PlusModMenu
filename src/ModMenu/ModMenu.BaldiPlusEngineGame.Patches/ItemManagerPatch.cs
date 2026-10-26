using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class ItemManagerPatch
{
	[HarmonyPatch(typeof(ItemManager), "RemoveItem")]
	private class RemoveItem
	{
		private static bool Prefix()
		{
			return !Service.Get<IModMenuService>().Get<InventoryModifiers>().InfiniteItems;
		}
	}

	[HarmonyPatch(typeof(ItemManager), "SetItem")]
	private class SetItem
	{
		private static bool Prefix(ItemManager __instance, ItemObject item, int slot)
		{
			if (item.item.GetType() == typeof(ITM_GrapplingHook) && __instance.items[slot]?.item.GetType() == typeof(ITM_GrapplingHook))
			{
				return !Service.Get<IModMenuService>().Get<InventoryModifiers>().InfiniteItems;
			}
			return true;
		}
	}
}
