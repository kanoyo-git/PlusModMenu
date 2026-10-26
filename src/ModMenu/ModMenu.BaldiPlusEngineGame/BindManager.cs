using System;
using System.Linq;
using PlusModMenu.Core.Unity;
using ModMenu.BaldiPlusEngineGame.Clipboard;
using ModMenu.BaldiPlusEngineGame.Settings;
using ModMenu.Settings;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame;

internal static class BindManager
{
	static BindManager()
	{
		InputHelper.KeyDownListener += Press;
	}

	public static void Init()
	{
	}

	public static KeyCode? KeyOf(SettingEntry entry)
	{
		return SettingBinds.CanBind(entry) && Config.Instance.Binds.TryGetValue(SettingBinds.Id(entry), out var key) ? key : null;
	}

	public static void Capture(SettingEntry entry, Action<int> tick, Action done)
	{
		KeyCapture.Start(tick, key =>
		{
			if (key.HasValue)
			{
				Config.Instance.SetBind(SettingBinds.Id(entry), key == KeyCode.Escape ? null : key);
			}
			done();
		}, passEscape: true);
	}

	internal static void Press(KeyCode key)
	{
		var binds = Config.Instance.Binds;
		if (binds.Count == 0 || !binds.ContainsValue(key) || ClipboardMenu.BlocksGame || KeyCapture.IsActive)
		{
			return;
		}
		foreach (var entry in SettingsRegistryService.Registry.AllEntries.ToList())
		{
			if (!entry.Visible || !binds.TryGetValue(SettingBinds.Id(entry), out var bound) || bound != key)
			{
				continue;
			}
			try
			{
				if (SettingBinds.Activate(entry))
				{
					BindToast.Show(entry);
				}
			}
			catch (Exception e)
			{
				Debug.LogError($"Plus Mod Menu: the key {key} of {SettingBinds.Id(entry)} failed: {e}");
			}
		}
	}
}
