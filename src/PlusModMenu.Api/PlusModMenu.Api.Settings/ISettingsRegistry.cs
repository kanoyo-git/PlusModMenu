using System;
using System.Collections.Generic;
using PlusModMenu.Api.Interfaces;

namespace PlusModMenu.Api.Settings;

public readonly struct NumberOptions
{
	public bool Integer { get; init; }

	public double? Min { get; init; }

	public double? Max { get; init; }

	public double? Step { get; init; }
}

public interface ISettingHandle
{
	UIText Label { get; set; }

	bool Visible { get; set; }

	void SetAvailable(bool available, UIText reason = default);

	void Refresh();

	void Remove();
}

public interface ISettingsGroup
{
	UIText Label { get; }

	ISettingHandle AddButton(UIText label, Action onClick, UIText help = default);

	ISettingHandle AddButton(UIText label, Action onClick, int order, UIText help = default);

	ISettingHandle AddToggle(UIText label, Func<bool> get, Action<bool> set, UIText help = default);

	ISettingHandle AddNumber(UIText label, Func<double> get, Action<double> set, NumberOptions options, UIText help = default);

	ISettingHandle AddChoice(UIText label, Func<IReadOnlyList<UIText>> options, Func<int> get, Action<int> set, UIText help = default);

	ISettingHandle AddText(UIText label, Func<string> get, Action<string> set, UIText help = default);

	void Remove();
}

public interface ISettingsPage
{
	UIText Label { get; }

	ISettingsGroup GetGroup(Type staticGroupType);

	ISettingsGroup AddGroup(UIText label);
}

public interface ISettingsRegistry : IService
{
	ISettingsPage GetPage(Type staticMenuType);

	ISettingsGroup GetGroup(Type staticGroupType);

	void Refresh(object modifiers, string memberName);

	void RefreshAll();

	IDialogs Dialogs { get; }
}
