using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Patches;
using ModMenu.BaldiPlusEngineGame.Variables;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Hl1;

internal static class GravityGun
{
	public const string NameKey = "ModMenu_Itm_GravityGun";

	public const string DescKey = "ModMenu_Desc_GravityGun";

	public const Items ItemType = (Items)60;

	private static ItemObject? _item;

	public static ItemObject Item
	{
		get
		{
			if (_item == null)
			{
				_item = Create();
			}
			return _item;
		}
	}

	private static ItemObject Create()
	{
		var prefab = new GameObject("ITM_GravityGun");
		prefab.SetActive(false);
		Object.DontDestroyOnLoad(prefab);
		prefab.AddComponent<ITM_GravityGun>();
		var item = ScriptableObject.CreateInstance<ItemObject>();
		item.name = "GravityGun";
		item.itemType = ItemType;
		item.nameKey = NameKey;
		item.descKey = DescKey;
		item.item = prefab.GetComponent<ITM_GravityGun>();
		item.itemSpriteSmall = Icon("ModMenu.gravity_gun_small.png", 1f);
		item.itemSpriteLarge = Icon("ModMenu.gravity_gun_large.png", 50f);
		item.value = 50;
		item.price = 500;
		item.addToInventory = true;
		item.overrideDisabled = true;
		Object.DontDestroyOnLoad(item);
		return item;
	}

	private static Sprite Icon(string resource, float pixelsPerUnit)
	{
		using var stream = typeof(GravityGun).Assembly.GetManifestResourceStream(resource);
		var bytes = new byte[stream!.Length];
		stream.Read(bytes, 0, bytes.Length);
		var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point, name = resource };
		texture.LoadImage(bytes);
		return UnityEngine.Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
	}

	public static void Give()
	{
		var player = Singleton<CoreGameManager>.Instance?.GetPlayer(0);
		if (player == null)
		{
			return;
		}
		player.itm.AddItem(Item);
	}

	internal static bool OnMap => Service.Get<IModMenuService>().Get<InventoryModifiers>().GravityGunOnMap;

	[HarmonyPatch(typeof(LevelGenerator), nameof(LevelGenerator.Generate))]
	private class AddToFloor
	{
		private static void Prefix(LevelGenerator __instance)
		{
			var ld = __instance.ld;
			if (ld == null)
			{
				return;
			}
			var items = (ld.forcedItems ?? new List<ItemObject>()).Where(x => x == null || x.itemType != ItemType).ToList();
			if (OnMap)
			{
				items.Add(Item);
			}
			ld.forcedItems = items;
		}
	}

	[HarmonyPatch(typeof(LocalizationManager), nameof(LocalizationManager.GetLocalizedText), typeof(string), typeof(bool))]
	private class Text
	{
		private static bool Prefix(string key, ref string __result)
		{
			switch (key)
			{
				case NameKey:
					__result = Strings.GRAVITY_GUN;
					return false;
				case DescKey:
					__result = HelpText.GRAVITY_GUN_DESC;
					return false;
				default:
					return true;
			}
		}
	}
}

internal class ITM_GravityGun : Item
{
	public override bool Use(PlayerManager pm)
	{
		var carry = pm.GetComponent<CarryController>() ?? CarryController.Attach(pm, Hl1Mode.Controller);
		carry.UseGravityGun();
		Destroy(gameObject);
		return false;
	}
}

internal static class GravityGunLevels
{
	public static void Init()
	{
		CoreGameManagerPatch.PlayersSpawned += _ =>
		{
			if (!Hl1Mode.Active)
			{
				LevelSolids.Clear();
			}
		};
	}
}
