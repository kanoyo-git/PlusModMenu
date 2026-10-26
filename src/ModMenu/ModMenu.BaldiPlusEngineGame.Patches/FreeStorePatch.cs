using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables.OtherModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal static class FreeStorePatch
{
	private static readonly FieldInfo _pickups = AccessTools.Field(typeof(StoreRoomFunction), "pickups");

	private static readonly FieldInfo _tags = AccessTools.Field(typeof(StoreRoomFunction), "tag");

	private static readonly FieldInfo _stickerPickups = AccessTools.Field(typeof(StoreRoomFunction), "stickerPickup");

	private static readonly FieldInfo _stickerTags = AccessTools.Field(typeof(StoreRoomFunction), "stickerTag");

	private static readonly FieldInfo _mapPickup = AccessTools.Field(typeof(StoreRoomFunction), "mapPickup");

	private static readonly FieldInfo _mapTag = AccessTools.Field(typeof(StoreRoomFunction), "mapTag");

	private static readonly FieldInfo _gluePickup = AccessTools.Field(typeof(StoreRoomFunction), "glueStickPickup");

	private static readonly FieldInfo _glueTag = AccessTools.Field(typeof(StoreRoomFunction), "glueStickTag");

	private static readonly FieldInfo _storeData = AccessTools.Field(typeof(StoreRoomFunction), "storeData");

	private static readonly FieldInfo _open = AccessTools.Field(typeof(StoreRoomFunction), "open");

	private static readonly MethodInfo _itemPurchased = AccessTools.Method(typeof(StoreRoomFunction), "ItemPurchased");

	private static readonly Dictionary<Pickup, int> _discounted = new();

	internal static bool On => Service.Get<IModMenuService>().Get<WorldModifiers>().FreeStore;

	internal static void ApplyAll(bool on)
	{
		foreach (var store in Object.FindObjectsOfType<StoreRoomFunction>())
		{
			if (on) Apply(store);
			else Restore(store);
		}
	}

	private static IEnumerable<(Pickup pickup, PriceTag tag)> Shelves(StoreRoomFunction store)
	{
		var pickups = (List<Pickup>)_pickups.GetValue(store);
		var tags = (PriceTag[])_tags.GetValue(store);
		for (var i = 0; i < pickups.Count && i < tags.Length; i++) yield return (pickups[i], tags[i]);
		var stickers = (Pickup[])_stickerPickups.GetValue(store);
		var stickerTags = (PriceTag[])_stickerTags.GetValue(store);
		for (var i = 0; i < stickers.Length && i < stickerTags.Length; i++) yield return (stickers[i], stickerTags[i]);
		yield return ((Pickup)_mapPickup.GetValue(store), (PriceTag)_mapTag.GetValue(store));
		yield return ((Pickup)_gluePickup.GetValue(store), (PriceTag)_glueTag.GetValue(store));
	}

	private static void Apply(StoreRoomFunction store)
	{
		foreach (var (pickup, tag) in Shelves(store))
		{
			if (pickup == null || tag == null || !pickup.gameObject.activeSelf || pickup.price <= 0) continue;
			_discounted[pickup] = pickup.price;
			tag.SetSale(pickup.price, 0);
			pickup.price = 0;
		}
	}

	private static void Restore(StoreRoomFunction store)
	{
		foreach (var (pickup, tag) in Shelves(store))
		{
			if (pickup == null || tag == null || !_discounted.TryGetValue(pickup, out var price)) continue;
			_discounted.Remove(pickup);
			if (!pickup.gameObject.activeSelf || !pickup.showDescription) continue;
			pickup.price = price;
			tag.SetText(price.ToString());
		}
	}

	[HarmonyPatch(typeof(StoreRoomFunction), nameof(StoreRoomFunction.Initialize))]
	private class Initialize
	{
		private static void Postfix(StoreRoomFunction __instance)
		{
			if (On && _storeData.GetValue(__instance) != null) Apply(__instance);
		}
	}

	[HarmonyPatch(typeof(StoreRoomFunction), "Restock")]
	private class Restock
	{
		private static void Prefix(StoreRoomFunction __instance)
		{
			foreach (var (pickup, _) in Shelves(__instance))
			{
				if (pickup != null) _discounted.Remove(pickup);
			}
		}

		private static void Postfix(StoreRoomFunction __instance)
		{
			if (On) Apply(__instance);
		}
	}

	[HarmonyPatch(typeof(StoreRoomFunction), nameof(StoreRoomFunction.PurchaseRestock))]
	private class PurchaseRestock
	{
		private static void Prefix(ref int ___restockPrice, out int __state)
		{
			__state = ___restockPrice;
			if (On) ___restockPrice = 0;
		}

		private static void Finalizer(ref int ___restockPrice, int __state) => ___restockPrice = __state;
	}

	[HarmonyPatch(typeof(StoreRoomFunction), "ItemCollected")]
	private class ItemCollected
	{
		private static void Postfix(StoreRoomFunction __instance, Pickup pickup, int player)
		{
			if (pickup != null && _discounted.Remove(pickup) && (bool)_open.GetValue(__instance))
			{
				_itemPurchased.Invoke(__instance, new object[] { pickup, player });
			}
		}
	}
}
