using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Collectors;
using ModMenu.BaldiPlusEngineGame.Settings;
using ModMenu.BaldiPlusEngineGame.Utils;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Collectors;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using PlusModMenu.Api.Settings;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables;

[DataContract]
public class InventoryModifiers : IModifiers
{
	[DataMember]
	[Label<Strings>("INFINITE_ITEMS")]
	[Help<HelpText>("INFINITE_ITEMS_TITLE", "INFINITE_ITEMS_TEXT")]
	[Group<ItemModifiersGeneralGroup>]
	public virtual bool InfiniteItems { get; set; }

	[DataMember]
	[Label<Strings>("USE_ITEMS_ANYWHERE")]
	[Help<HelpText>("USE_ITEMS_ANYWHERE_TITLE", "USE_ITEMS_ANYWHERE_TEXT")]
	[Group<ItemModifiersGeneralGroup>]
	public virtual bool UseItemsAnywhere { get; set; }

	[DataMember]
	[Label<Strings>("GRAVITY_GUN_ON_MAP")]
	[Help<HelpText>("GRAVITY_GUN_ON_MAP_TITLE", "GRAVITY_GUN_ON_MAP_TEXT")]
	[Group<ItemModifiersGeneralGroup>]
	public virtual bool GravityGunOnMap { get; set; }

	public InventoryModifiers()
	{
		Service.Get<IEventService>().SubscribeOnce<IModMenuInitializedEvent>((object? _) =>
		{
			GenerateInventoryEditorUI();
		});
	}

	private static readonly FieldInfo _loseableItems = AccessTools.Field(typeof(Structure_StudentSpawner), "loseableItem");

	internal static List<ItemObject> StudentToys()
	{
		var toys = Resources.FindObjectsOfTypeAll<Structure_StudentSpawner>()
			.SelectMany(x => (ItemObject[])_loseableItems.GetValue(x) ?? new ItemObject[0])
			.Concat(Resources.FindObjectsOfTypeAll<ItemObject>().Where(x => x.itemType >= Items.lostItem0 && x.itemType <= Items.lostItem9));
		return toys.Where(x => x != null && x.item != null).Distinct(new ItemObjectEqualityComparer()).OrderBy(x => x.itemType).ToList();
	}

	internal static List<ItemObject> Giveable(List<ItemObject> seen)
	{
		var comparer = new ItemObjectEqualityComparer();
		var choices = new List<ItemObject>(seen);
		choices.AddRange(StudentToys().Where(toy => !choices.Contains(toy, comparer)));
		return choices;
	}

	internal static List<ItemObject> SeenItems { get; } = new List<ItemObject>();

	private static readonly List<ISettingHandle> _slotButtons = new List<ISettingHandle>();

	internal static IEnumerable<string> SlotLabels => _slotButtons.Select(x => x.Label.Resolve());

	internal static void NameSlots()
	{
		ItemManager? itm = Singleton<CoreGameManager>.Instance != null ? Singleton<CoreGameManager>.Instance.GetPlayer(0)?.itm : null;
		for (int i = 0; i < _slotButtons.Count; i++)
		{
			ItemObject? item = itm != null && i < itm.items.Length ? itm.items[i] : null;
			_slotButtons[i].Label = item != null && item.itemType != Items.None
				? UIText.Literal($"{i + 1}. {EnglishGameText.Get(item.nameKey)}")
				: UIText.Resource<Strings>("EDIT_SLOT__INDEX", i + 1);
		}
	}

	[HarmonyPatch(typeof(HudManager), nameof(HudManager.UpdateItemIcon))]
	private static class SlotNames
	{
		private static void Postfix() => NameSlots();
	}

	private static void GenerateInventoryEditorUI()
	{
		ISettingsRegistry registry = SettingsRegistryService.Registry;
		ISettingHandle giveGun = registry.GetGroup(typeof(ItemModifiersGeneralGroup)).AddButton(UIText.Resource<Strings>("GIVE_GRAVITY_GUN"), Hl1.GravityGun.Give, UIText.Resource<HelpText>("GIVE_GRAVITY_GUN_TEXT"));
		ISettingsGroup group = registry.GetPage(typeof(ItemModifiersGeneralMenu)).AddGroup(UIText.Resource<Strings>("INVENTORY_SLOT_EDITOR"));
		UIText notInGame = UIText.Resource<Strings>("NOT_AVAILABLE_NOW");
		List<ItemObject> items = SeenItems;
		List<ISettingHandle> editSlotButtons = new List<ISettingHandle>();
		for (int i = 0; i < 9; i++)
		{
			int index = i;
			ISettingHandle button = group.AddButton(UIText.Resource<Strings>("EDIT_SLOT__INDEX", i + 1), () =>
			{
				List<ItemObject> choices = Giveable(items);
				List<UIText> names = choices.ConvertAll(x => UIText.Literal(EnglishGameText.Get(x.nameKey)));
				registry.Dialogs.Choose(UIText.Resource<Strings>("SELECT_ITEM"), names, chosen =>
				{
					ItemManager itm = Singleton<CoreGameManager>.Instance?.GetPlayer(0)?.itm;
					if (itm != null && index <= itm.maxItem)
					{
						itm.SetItem(choices[chosen], index);
					}
				});
			});
			button.SetAvailable(available: false, notInGame);
			editSlotButtons.Add(button);
			_slotButtons.Add(button);
		}
		giveGun.SetAvailable(available: false, notInGame);
		editSlotButtons.Add(giveGun);
		Service.Get<IEventService>().Subscribe<ISceneChangedEvent>((object? x) =>
		{
			foreach (ISettingHandle button in editSlotButtons)
			{
				button.SetAvailable(ModMenuManager.IsAnyGameScene(x), notInGame);
			}
			NameSlots();
		});
		DistinctSingletonCollector<ItemObject, ItemObjectEqualityComparer>.ItemAdded += (ItemObject x) =>
		{
			items.Add(x);
		};
		DistinctSingletonCollector<ItemObject, ItemObjectEqualityComparer>.ItemRemoved += (ItemObject x) =>
		{
			items.Remove(x);
		};
	}
}
