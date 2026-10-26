using System;
using System.Collections.Generic;
using System.Linq;
using PlusModMenu.Core.Unity;
using ModMenu.BaldiPlusEngineGame.Settings;
using ModMenu.Settings;
using PlusModMenu.Api;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces.Services;
using PlusModMenu.Api.Settings;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame;

internal static class KeptManager
{
	private static readonly HashSet<string> _applied = new HashSet<string>();

	internal static int Applied { get; private set; }

	public static bool AltHeld => Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);

	public static bool IsKept(SettingEntry entry)
	{
		return SettingKeep.CanKeep(entry) && Config.Instance.Kept.Contains(SettingBinds.Id(entry));
	}

	public static void Flip(SettingEntry entry)
	{
		var kept = Config.Instance.FlipKept(entry);
		_applied.Add(SettingBinds.Id(entry));
		var name = entry.Label.Resolve();
		BindToast.Show(UIText.Resource<Strings>(kept ? "KEPT_TOAST__NAME" : "UNKEPT_TOAST__NAME", name).Resolve());
		entry.Label = entry.Label;
	}

	public static void Init()
	{
		Apply();
		Service.Get<IEventService>().Subscribe<ISceneChangedEvent>((object? _) => Apply());
	}

	private static void Apply()
	{
		var kept = Config.Instance.Kept;
		if (kept.Count == 0)
		{
			return;
		}
		foreach (var entry in SettingsRegistryService.Registry.AllEntries.ToList())
		{
			try
			{
				Applied += SettingKeep.Apply(new[] { entry }, kept, _applied);
			}
			catch (Exception e)
			{
				Debug.LogError($"Plus Mod Menu: could not switch on the kept {SettingBinds.Id(entry)}: {e}");
			}
		}
	}
}
