using ModMenu.BaldiPlusEngineGame.Clipboard;
using System;
using System.Collections.Generic;
using PlusModMenu.Core.Common;
using PlusModMenu.Core.Unity;
using ModMenu.BaldiPlusEngineGame.Internal.Menus;
using ModMenu.BaldiPlusEngineGame.Settings;
using ModMenu.Common;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using PlusModMenu.Api.Settings;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame;

internal class HotkeyManager
{
	private static readonly List<JToken> _savedStates;

	private static JToken? _initialState;

	private static Hotkey[] _hotkeys;

	static HotkeyManager()
	{
		_savedStates = new List<JToken>();
		_hotkeys = Array.Empty<Hotkey>();
		InputHelper.KeyDownListener += OnKeyDown;
	}

	private static void OnKeyDown(KeyCode key)
	{
		if (ClipboardMenu.BlocksGame)
		{
			return;
		}
		Hotkey[] hotkeys = _hotkeys;
		foreach (Hotkey hotkey in hotkeys)
		{
			if (hotkey.Key == key)
			{
				hotkey.Run();
			}
		}
	}

	public static void Init()
	{
		_hotkeys = Hotkey.GetHotkeysFromDir(DataPaths.GetPath("Hotkeys"));
		GenerateUI();
	}

	public static void StartRecording()
	{
		if (_initialState != null)
		{
			Debugger.Error("A recording has already been started.");
		}
		else
		{
			SaveInitialState();
		}
	}

	public static void Reset()
	{
		if (_initialState == null)
		{
			Debugger.Error("A recording has not been started.");
			return;
		}
		_initialState = null;
		_savedStates.Clear();
	}

	public static void SaveState(bool final)
	{
		if (_initialState == null)
		{
			Debugger.Error("A recording has not been started.");
			return;
		}
		JArray next = JArray.Parse(PresetSerializer.Serialize(Service.Get<IModMenuService>().Modifiers));
		JToken difference = GetDifference(_initialState, next);
		if (difference != null)
		{
			_savedStates.Add(difference);
			SaveInitialState();
		}
		else if (!final)
		{
			SettingsRegistryService.Registry.Dialogs.Message(UIText.Resource<Strings>("NOTICE"), UIText.Resource<Strings>("HOTKEY_NO_RECORDABLE_CHANGES_NOTICE"));
		}
	}

	private static JToken? GetDifference(JToken? prev, JToken? next)
	{
		if (JToken.DeepEquals(prev, next))
		{
			return null;
		}
		if (!(prev is JArray jArray))
		{
			if (prev is JObject jObject && next is JObject jObject2)
			{
				JObject jObject3 = new JObject();
				foreach (KeyValuePair<string, JToken> item in jObject)
				{
					JToken value = item.Value;
					bool flag;
					if (value != null)
					{
						JTokenType type = value.Type;
						if ((uint)(type - 1) <= 1u)
						{
							flag = true;
							goto IL_00c9;
						}
					}
					flag = false;
					goto IL_00c9;
					IL_00c9:
					if (flag)
					{
						JToken difference = GetDifference(jObject[item.Key], jObject2[item.Key]);
						if (difference != null)
						{
							jObject3[item.Key] = difference;
						}
					}
					else if (!JToken.DeepEquals(jObject[item.Key], jObject2[item.Key]) || string.Equals(item.Key, "$type", StringComparison.Ordinal))
					{
						jObject3[item.Key] = jObject2[item.Key];
					}
					jObject2.Remove(item.Key);
				}
				foreach (KeyValuePair<string, JToken> item2 in jObject2)
				{
					jObject3[item2.Key] = jObject2[item2.Key];
				}
				if (jObject3.Count <= 0)
				{
					return null;
				}
				return jObject3;
			}
		}
		else if (next is JArray jArray2)
		{
			JArray jArray3 = new JArray();
			for (int i = 0; i < jArray.Count; i++)
			{
				JToken difference2 = GetDifference(jArray[i], jArray2[i]);
				if (difference2 != null)
				{
					jArray3.Add(difference2);
				}
			}
			return jArray3;
		}
		return null;
	}

	private static void SaveInitialState()
	{
		_initialState = JArray.Parse(PresetSerializer.Serialize(Service.Get<IModMenuService>().Modifiers));
	}

	public static void GenerateUI()
	{
		ISettingsRegistry registry = SettingsRegistryService.Registry;
		ISettingsGroup panel = registry.GetGroup(typeof(SideMenuGroupModMenuHotkeyControlPanel));
		ISettingsPage hotkeysPage = registry.GetPage(typeof(SideMenuMenuHotkeys));
		List<ISettingsGroup> hotkeyGroups = new List<ISettingsGroup>();
		UIText notRecording = UIText.Resource<Strings>("HOTKEY_NOT_RECORDING");
		ISettingHandle createHotkey = null;
		ISettingHandle saveState = null;
		bool currentlyRecording = false;
		panel.AddButton(UIText.Resource<Strings>("REFRESH_HOTKEYS"), RefreshHotkeyList);
		createHotkey = panel.AddButton(UIText.Resource<Strings>("CREATE_NEW_HOTKEY"), () =>
		{
			if (!currentlyRecording)
			{
				StartRecording();
				currentlyRecording = true;
				createHotkey.Label = UIText.Resource<Strings>("SAVE_HOTKEY");
				saveState.SetAvailable(available: true);
				return;
			}
			currentlyRecording = false;
			createHotkey.Label = UIText.Resource<Strings>("CREATE_NEW_HOTKEY");
			saveState.SetAvailable(available: false, notRecording);
			SaveState(final: true);
			if (_savedStates.Count == 0)
			{
				Reset();
				registry.Dialogs.Message(UIText.Resource<Strings>("NOTICE"), UIText.Resource<Strings>("HOTKEY_NO_RECORDABLE_CHANGES_NOTICE"));
				return;
			}
			registry.Dialogs.Prompt(UIText.Resource<Strings>("CREATE_NEW_HOTKEY"), UIText.Resource<Strings>("HOTKEY_NAME"), string.Empty, UIText.Resource<Strings>("SAVE"), name =>
			{
				Hotkey.Create(name, _savedStates);
				Reset();
				RefreshHotkeyList();
			}, Reset);
		}, UIText.Resource<HelpText>("HOTKEYS_TEXT"));
		saveState = panel.AddButton(UIText.Resource<Strings>("SAVE_HOTKEY_STATE"), () =>
		{
			SaveState(final: false);
		});
		saveState.SetAvailable(available: false, notRecording);
		RefreshHotkeyList();
		void RefreshHotkeyList()
		{
			foreach (ISettingsGroup group in hotkeyGroups)
			{
				group.Remove();
			}
			hotkeyGroups.Clear();
			_hotkeys = Hotkey.GetHotkeysFromDir(DataPaths.GetPath("Hotkeys"));
			foreach (Hotkey hotkey in _hotkeys)
			{
				ISettingsGroup group = hotkeysPage.AddGroup(UIText.Literal(hotkey.Name));
				group.AddToggle(UIText.Resource<Strings>("ENABLED"), () => hotkey.Enabled, x =>
				{
					hotkey.Enabled = x;
				});
				ISettingHandle keyButton = null;
				keyButton = group.AddButton(UIText.Resource<Strings>("HOTKEY__KEY", hotkey.Key), () => KeyCapture.Start(seconds =>
				{
					keyButton.Label = UIText.Resource<Strings>("WAITING_FOR_INPUT__TIME", seconds);
				}, key =>
				{
					if (key.HasValue)
					{
						hotkey.Key = key.Value;
					}
					keyButton.Label = UIText.Resource<Strings>("HOTKEY__KEY", hotkey.Key);
				}));
				group.AddButton(UIText.Resource<Strings>("HOTKEY_INFORMATION"), () =>
				{
					hotkey.Load();
					registry.Dialogs.Message(UIText.Resource<Strings>("HOTKEY_INFORMATION"), UIText.Literal(hotkey.ToString()));
				});
				group.AddButton(UIText.Resource<Strings>("DELETE_HOTKEY"), () =>
				{
					registry.Dialogs.Confirm(UIText.Resource<Strings>("DELETE_HOTKEY"), UIText.Resource<Strings>("DELETE_HOTKEY_CONFIRM", hotkey.Name), UIText.Resource<Strings>("DELETE_HOTKEY"), () =>
					{
						hotkey.Delete();
						RefreshHotkeyList();
					});
				});
				hotkeyGroups.Add(group);
			}
		}
	}
}
