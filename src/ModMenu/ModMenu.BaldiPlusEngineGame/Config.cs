using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PlusModMenu.Core.Common;
using PlusModMenu.Core.Unity;
using ModMenu.BaldiPlusEngineGame.Internal;
using ModMenu.BaldiPlusEngineGame.Internal.Menus;
using ModMenu.BaldiPlusEngineGame.Clipboard;
using ModMenu.BaldiPlusEngineGame.Settings;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces;
using ModMenu.Settings;
using PlusModMenu.Api.Internal;
using PlusModMenu.Api.Settings;
using BepInEx.Configuration;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame;

internal class Config
{
	private const string MenuSection = "Menu";

	private const string KeysSection = "Keys";

	private const string RowsSection = "Menu rows";

	public static Config Instance { get; private set; } = null!;

	private readonly ConfigEntry<KeyboardShortcut> _modMenuKey;

	private readonly ConfigEntry<MenuLanguage> _menuLanguage;

	private readonly ConfigEntry<float> _characterSpawnActivation;

	private readonly ConfigEntry<KeyboardShortcut> _jumpKey;

	private readonly ConfigEntry<KeyboardShortcut> _hl1DuckKey;

	private readonly ConfigEntry<KeyboardShortcut> _hl1UseKey;

	private readonly ConfigEntry<string> _bindsText;

	private readonly ConfigEntry<string> _keptText;

	private readonly Dictionary<string, KeyCode> _binds;

	private readonly List<string> _kept;

	private Config(ConfigFile file)
	{
		_modMenuKey = file.Bind(MenuSection, "Open key", new KeyboardShortcut(KeyCode.Tab), "Key that opens and closes the menu.");
		_menuLanguage = file.Bind(MenuSection, "Language", MenuLanguage.Auto, "Menu language. Auto: Russian when CyrillicPlus is installed, English otherwise.");
		_characterSpawnActivation = file.Bind(MenuSection, "Character activation distance", 5f, "A character spawned from the menu stays see-through and harmless until the player is this many tiles away.");
		_jumpKey = file.Bind(KeysSection, "Jump", new KeyboardShortcut(KeyCode.Mouse0), "Jump key for Jump Anywhere and HL1 Movement.");
		_hl1DuckKey = file.Bind(KeysSection, "HL1 duck", new KeyboardShortcut(KeyCode.LeftControl), "Duck key for HL1 Movement.");
		_hl1UseKey = file.Bind(KeysSection, "HL1 use", new KeyboardShortcut(KeyCode.E), "Pick up and drop props in HL1 Movement and with the Gravity Gun.");
		_bindsText = file.Bind(RowsSection, "Binds", string.Empty, "Keys bound to menu rows (right-click a row in the menu): row = key; row = key.");
		_keptText = file.Bind(RowsSection, "Kept on", string.Empty, "Toggles that stay on after a restart (Alt+click a toggle in the menu), separated by ;.");
		_binds = new Dictionary<string, KeyCode>();
		foreach (var bind in ConfigText.ParseMap(_bindsText.Value))
		{
			if (Enum.IsDefined(typeof(KeyCode), bind.Value) && Enum.Parse(typeof(KeyCode), bind.Value) is KeyCode key)
			{
				_binds[Renamed(bind.Key)] = key;
			}
		}
		_kept = ConfigText.ParseList(_keptText.Value).Select(Renamed).Distinct().ToList();
		Internals.CharacterActivationDistance = _characterSpawnActivation.Value;
	}

	public static void Init(ConfigFile file)
	{
		Instance = new Config(file);
		Instance.MigrateJson();
		Instance.SaveRows();
	}

	private void MigrateJson()
	{
		string path = Path.Combine(DataPaths.GetPath("Mod Directory"), "config.json");
		if (!File.Exists(path))
		{
			return;
		}
		try
		{
			JObject json = JObject.Parse(File.ReadAllText(path));
			SetFromJson(json, "ModMenuKey", _modMenuKey, x => new KeyboardShortcut((KeyCode)(int)x));
			SetFromJson(json, "MenuLanguage", _menuLanguage, x => (MenuLanguage)(int)x);
			SetFromJson(json, "CharacterSpawnActivation", _characterSpawnActivation, x => (float)x);
			SetFromJson(json, "JumpKey", _jumpKey, x => new KeyboardShortcut((KeyCode)(int)x));
			SetFromJson(json, "Hl1DuckKey", _hl1DuckKey, x => new KeyboardShortcut((KeyCode)(int)x));
			SetFromJson(json, "Hl1UseKey", _hl1UseKey, x => new KeyboardShortcut((KeyCode)(int)x));
			if (json["Binds"] is JObject binds)
			{
				foreach (JProperty bind in binds.Properties())
				{
					_binds[Renamed(bind.Name)] = (KeyCode)(int)bind.Value;
				}
			}
			if (json["Kept"] is JArray kept)
			{
				_kept.AddRange(kept.Select(x => Renamed((string)x)).Where(x => !_kept.Contains(x)));
			}
			Internals.CharacterActivationDistance = _characterSpawnActivation.Value;
			Debug.Log("Moved menu settings from " + path + " to the BepInEx config");
		}
		catch (Exception e)
		{
			Debug.LogWarning("Could not read the old " + path + ", it is removed: " + e.Message);
		}
		File.Delete(path);
	}

	private static void SetFromJson<T>(JObject json, string name, ConfigEntry<T> entry, Func<JToken, T> convert)
	{
		if (json[name] is JToken token && token.Type != JTokenType.Null)
		{
			entry.Value = convert(token);
		}
	}

	private void SaveRows()
	{
		_bindsText.Value = ConfigText.FormatMap(_binds.Select(x => new KeyValuePair<string, string>(x.Key, x.Value.ToString())));
		_keptText.Value = ConfigText.FormatList(_kept);
	}

	public KeyCode JumpKey
	{
		get => _jumpKey.Value.MainKey;
		set => _jumpKey.Value = new KeyboardShortcut(value);
	}

	public Dictionary<string, KeyCode> Binds => _binds;

	public List<string> Kept => _kept;

	private static readonly (string from, string to)[] RenamedRows = { ("/FastResults", "/FastElevator") };

	private static string Renamed(string id)
	{
		foreach (var (from, to) in RenamedRows)
		{
			if (id.EndsWith(from, StringComparison.Ordinal)) return id.Substring(0, id.Length - from.Length) + to;
		}
		return id;
	}

	public bool FlipKept(SettingEntry entry)
	{
		var kept = SettingKeep.Flip(_kept, entry);
		SaveRows();
		return kept;
	}

	public void SetBind(string id, KeyCode? key)
	{
		if (key.HasValue)
		{
			_binds[id] = key.Value;
		}
		else
		{
			_binds.Remove(id);
		}
		SaveRows();
	}

	public KeyCode Hl1DuckKey
	{
		get => _hl1DuckKey.Value.MainKey;
		set => _hl1DuckKey.Value = new KeyboardShortcut(value);
	}

	public KeyCode Hl1UseKey
	{
		get => _hl1UseKey.Value.MainKey;
		set => _hl1UseKey.Value = new KeyboardShortcut(value);
	}

	public KeyCode ModMenuKey
	{
		get => _modMenuKey.Value.MainKey;
		set => _modMenuKey.Value = new KeyboardShortcut(value);
	}

	public float CharacterSpawnActivation
	{
		get => _characterSpawnActivation.Value;
		set
		{
			_characterSpawnActivation.Value = value;
			Internals.CharacterActivationDistance = value;
		}
	}

	public MenuLanguage MenuLanguage
	{
		get => _menuLanguage.Value;
		set => _menuLanguage.Value = value;
	}

	public static void GenerateUI()
	{
		ISettingsGroup group = SettingsRegistryService.Registry.GetGroup(typeof(SideMenuGroupModMenuSettingsGeneral));
		ISettingHandle keyButton = null;
		keyButton = group.AddButton(KeyLabel(), () => KeyCapture.Start(seconds =>
		{
			keyButton.Label = UIText.Resource<Strings>("WAITING_FOR_INPUT__TIME", seconds);
		}, key =>
		{
			if (key.HasValue)
			{
				Instance.ModMenuKey = key.Value;
			}
			keyButton.Label = KeyLabel();
		}));
		group.AddChoice(UIText.Resource<Strings>("LANGUAGE"), () => new List<UIText>
		{
			UIText.Resource<Strings>("LANGUAGE_AUTO"),
			UIText.Resource<Strings>("LANGUAGE_ENGLISH"),
			UIText.Resource<Strings>("LANGUAGE_RUSSIAN")
		}, () => (int)Instance.MenuLanguage, x =>
		{
			Instance.MenuLanguage = (MenuLanguage)x;
			Localization.Apply((MenuLanguage)x);
		}, UIText.Resource<HelpText>("LANGUAGE_TEXT"));
		ISettingHandle cyrillicWarning = group.AddButton(UIText.Resource<Strings>("RUSSIAN_NEEDS_CYRILLICPLUS"), () =>
		{
			SettingsRegistryService.Registry.Dialogs.Message(UIText.Resource<Strings>("RUSSIAN_NEEDS_CYRILLICPLUS"), UIText.Resource<Strings>("RUSSIAN_NEEDS_CYRILLICPLUS_TEXT"));
		});
		cyrillicWarning.Visible = Localization.MissingCyrillic;
		Localization.Changed += () =>
		{
			cyrillicWarning.Visible = Localization.MissingCyrillic;
		};
		group.AddNumber(UIText.Resource<Strings>("CHARACTER_ACTIVATION_DISTANCE"), () => Instance.CharacterSpawnActivation, x =>
		{
			Instance.CharacterSpawnActivation = (float)x;
		}, new NumberOptions
		{
			Min = 0.0
		});
		static UIText KeyLabel()
		{
			return UIText.Resource<Strings>("MODMENU_KEY__KEY", Instance.ModMenuKey);
		}
	}
}
