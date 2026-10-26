using System.Collections.Generic;
using PlusModMenu.Core.Unity;
using ModMenu.BaldiPlusEngineGame.Internal.Menus;
using ModMenu.BaldiPlusEngineGame.Settings;
using ModMenu.Common;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using PlusModMenu.Api.Settings;

namespace ModMenu.BaldiPlusEngineGame;

internal class Presets
{
	private static readonly List<ISettingsGroup> _presetGroups = new List<ISettingsGroup>();

	public static void Init()
	{
		ISettingsRegistry registry = SettingsRegistryService.Registry;
		ISettingsGroup group = registry.GetGroup(typeof(PresetsGroup));
		group.AddButton(UIText.Resource<Strings>("CREATE_NEW_PRESET"), () =>
		{
			registry.Dialogs.Prompt(UIText.Resource<Strings>("SAVE_CURRENT_PRESET"), UIText.Resource<Strings>("PRESET_NAME"), string.Empty, UIText.Resource<Strings>("SAVE"), name =>
			{
				if (!string.IsNullOrWhiteSpace(name))
				{
					Preset.Create(name, Service.Get<IModMenuService>().Modifiers);
					RefreshPresetList();
				}
			});
		});
		group.AddButton(UIText.Resource<Strings>("REFRESH_PRESETS"), RefreshPresetList);
		RefreshPresetList();
	}

	private static void RefreshPresetList()
	{
		ISettingsRegistry registry = SettingsRegistryService.Registry;
		foreach (ISettingsGroup group in _presetGroups)
		{
			group.Remove();
		}
		_presetGroups.Clear();
		ISettingsPage page = registry.GetPage(typeof(PresetsMenu));
		foreach (Preset preset in Preset.GetPresetsFromDir(DataPaths.GetPath("Presets")))
		{
			ISettingsGroup group = page.AddGroup(UIText.Literal(preset.Name));
			group.AddButton(UIText.Resource<Strings>("LOAD_PRESET"), () =>
			{
				preset.Load();
				registry.RefreshAll();
			});
			group.AddButton(UIText.Resource<Strings>("REMOVE"), () =>
			{
				registry.Dialogs.Confirm(UIText.Resource<Strings>("REMOVE"), UIText.Resource<Strings>("DELETE_PRESET_CONFIRM", preset.Name), UIText.Resource<Strings>("REMOVE"), () =>
				{
					preset.Delete();
					RefreshPresetList();
				});
			});
			_presetGroups.Add(group);
		}
	}
}
