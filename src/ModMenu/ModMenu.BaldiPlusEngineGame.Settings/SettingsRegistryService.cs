using System;
using System.Collections.Generic;
using ModMenu.Settings;
using PlusModMenu.Api.Settings;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Settings;

internal static class SettingsRegistryService
{
	private sealed class DialogsProxy : IDialogs
	{
		public IDialogs Target;

		private IDialogs Current
		{
			get
			{
				if (Target == null)
				{
					Debug.LogWarning("Plus Mod Menu: a dialog was requested before the menu was created.");
				}
				return Target;
			}
		}

		public void Message(UIText title, UIText text)
		{
			Current?.Message(title, text);
		}

		public void Confirm(UIText title, UIText text, UIText confirmLabel, Action onConfirm)
		{
			Current?.Confirm(title, text, confirmLabel, onConfirm);
		}

		public void Prompt(UIText title, UIText text, string initialValue, UIText confirmLabel, Action<string> onConfirm, Action onCancel = null)
		{
			Current?.Prompt(title, text, initialValue, confirmLabel, onConfirm, onCancel);
		}

		public void Choose(UIText title, IReadOnlyList<UIText> options, Action<int> onChoose)
		{
			Current?.Choose(title, options, onChoose);
		}
	}

	private static readonly DialogsProxy Dialogs = new DialogsProxy();

	public static SettingsRegistry Registry { get; } = new SettingsRegistry(Dialogs)
	{
		Log = Debug.LogException
	};

	public static void SetDialogs(IDialogs dialogs)
	{
		Dialogs.Target = dialogs;
	}
}
