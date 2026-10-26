using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Settings;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using PlusModMenu.Api.Settings;

namespace ModMenu.BaldiPlusEngineGame.Variables.OtherModifiers;

[DataContract]
internal class StickerModifiers : IModifiers
{
	private static readonly FieldInfo _slotUpgraded = AccessTools.Field(typeof(StickerManager), "slotUpgraded");

	private static readonly FieldInfo _stickerData = AccessTools.Field(typeof(StickerManager), "stickerData");

	private static readonly FieldInfo _onStickerApplied = AccessTools.Field(typeof(StickerManager), "OnStickerApplied");

	internal static readonly Sticker[] Hidden =
	{
		Sticker.Radar, Sticker.Forecast, Sticker.BurntLights, Sticker.ItemTimeBoost, Sticker.TotalItemValueBoost,
		Sticker.ActivityCompletion, Sticker.InstantNotebook, Sticker.NightVision, Sticker.CollisionFailChance,
		Sticker.Compass, Sticker.NpcSlowdown, Sticker.placeholder1, Sticker.placeholder2, Sticker.GlueStick,
		Sticker.EnumLength
	};

	internal static readonly Sticker[] Offered = Enum.GetValues(typeof(Sticker)).Cast<Sticker>().Where(x => !Hidden.Contains(x)).ToArray();

	private int _toAdd = Array.IndexOf(Offered, Sticker.Stamina);

	private static StickerManager? Manager => Singleton<StickerManager>.Instance;

	protected StickerModifiers()
	{
		Service.Get<IEventService>().SubscribeOnce<IModMenuInitializedEvent>((object? _) => GenerateUI());
	}

	private static UIText Title(Sticker sticker)
	{
		var key = $"StickerTitle_{sticker}";
		var localization = Singleton<LocalizationManager>.Instance;
		var text = localization != null ? localization.GetLocalizedText(key) : key;
		return sticker == Sticker.Nothing ? UIText.Resource<Strings>("STICKER_NONE") : UIText.Literal(string.IsNullOrEmpty(text) || text == key ? sticker.ToString() : text);
	}

	private static bool Known(StickerManager manager, Sticker sticker)
	{
		return _stickerData.GetValue(manager) is Array data && (int)sticker < data.Length;
	}

	private static void Applied(StickerManager manager)
	{
		(_onStickerApplied.GetValue(manager) as StickerManager.StickerAppliedDelegate)?.Invoke();
	}

	internal static void SetSlot(int slot, Sticker sticker)
	{
		var manager = Manager;
		if (manager == null || slot < 0 || slot >= manager.activeStickerData.Length || !Known(manager, sticker))
		{
			return;
		}
		var level = Singleton<BaseGameManager>.Instance != null ? Singleton<BaseGameManager>.Instance.CurrentLevel : 0;
		manager.activeStickerData[slot] = new StickerStateData(sticker, level, opened: true, sticky: false);
		Applied(manager);
	}

	internal static void SetGlued(int slot, bool glued)
	{
		var manager = Manager;
		if (manager != null && _slotUpgraded.GetValue(manager) is bool[] upgraded && slot >= 0 && slot < upgraded.Length)
		{
			upgraded[slot] = glued;
			Applied(manager);
		}
	}

	private void GenerateUI()
	{
		var registry = SettingsRegistryService.Registry;
		var notInGame = UIText.Resource<Strings>("NOT_AVAILABLE_NOW");
		var rows = new List<ISettingHandle>();
		IReadOnlyList<UIText> Options() => Offered.Select(Title).ToList();

		var slots = registry.GetGroup(typeof(SideMenuGroupOtherModifiersStickersSlots));
		for (var i = 0; i < 4; i++)
		{
			var slot = i;
			rows.Add(slots.AddChoice(UIText.Resource<Strings>("STICKER_SLOT", slot + 1), Options, () =>
			{
				var manager = Manager;
				var index = manager != null && manager.activeStickerData[slot] != null ? Array.IndexOf(Offered, manager.activeStickerData[slot].sticker) : 0;
				return Math.Max(index, 0);
			}, x => SetSlot(slot, Offered[x]), UIText.Resource<HelpText>("STICKER_SLOT_TEXT")));
			rows.Add(slots.AddToggle(UIText.Resource<Strings>("STICKER_GLUE", slot + 1), () => Manager != null && Manager.SlotUpgraded(slot), x => SetGlued(slot, x), UIText.Resource<HelpText>("STICKER_GLUE_TEXT")));
		}

		var inventory = registry.GetGroup(typeof(SideMenuGroupOtherModifiersStickersInventory));
		inventory.AddChoice(UIText.Resource<Strings>("STICKER_TO_ADD"), Options, () => _toAdd, x => _toAdd = x);
		rows.Add(inventory.AddButton(UIText.Resource<Strings>("STICKER_ADD"), () =>
		{
			var manager = Manager;
			if (manager != null && Offered[_toAdd] != Sticker.Nothing && Known(manager, Offered[_toAdd]))
			{
				manager.AddStickerToInventory(Offered[_toAdd], displayAnimation: Singleton<CoreGameManager>.Instance != null && Singleton<CoreGameManager>.Instance.GetHud(0) != null);
			}
		}, UIText.Resource<HelpText>("STICKER_ADD_TEXT")));
		rows.Add(inventory.AddButton(UIText.Resource<Strings>("STICKER_OPEN_PACKS"), () => Manager?.OpenUnopenedStickerPackets(animation: false)));

		foreach (var row in rows)
		{
			row.SetAvailable(available: false, notInGame);
		}
		Service.Get<IEventService>().Subscribe<ISceneChangedEvent>((object? x) =>
		{
			foreach (var row in rows)
			{
				row.SetAvailable(ModMenuManager.IsAnyGameScene(x) && Manager != null, notInGame);
				row.Refresh();
			}
		});
	}
}
