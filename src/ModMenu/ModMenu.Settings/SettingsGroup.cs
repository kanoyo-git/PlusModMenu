using System;
using System.Collections.Generic;
using System.Linq;
using PlusModMenu.Api.Settings;

namespace ModMenu.Settings;

public sealed class SettingsGroup : ISettingsGroup
{
	private readonly List<SettingEntry> _entries = new List<SettingEntry>();

	public Type StaticType { get; }

	public UIText Label { get; }

	public SettingsPage Page { get; }

	public OrderAttribute Order { get; }

	public bool IsRemoved { get; internal set; }

	public IReadOnlyList<SettingEntry> Entries => OrderRules.Sort(_entries.Where(e => e.Visible), e => e.Order);

	public IReadOnlyList<SettingEntry> AllEntries => _entries;

	internal SettingsGroup(SettingsPage page, Type staticType, UIText label, OrderAttribute order)
	{
		Page = page;
		StaticType = staticType;
		Label = label;
		Order = order;
	}

	public SettingEntry Add(SettingEntry entry)
	{
		if (entry.Group != this)
		{
			throw new ArgumentException("The entry belongs to another group.", nameof(entry));
		}
		_entries.Add(entry);
		Page.Registry.OnEntryAdded(entry);
		Page.Registry.RaisePageChanged(Page);
		return entry;
	}

	internal void RemoveEntry(SettingEntry entry)
	{
		if (!entry.IsRemoved && _entries.Remove(entry))
		{
			entry.IsRemoved = true;
			Page.Registry.OnEntryRemoved(entry);
			Page.Registry.RaisePageChanged(Page);
		}
	}

	public void Remove()
	{
		Page.RemoveGroup(this);
	}

	private SettingEntry AddManual(SettingKind kind, UIText label, UIText help, Func<object> get, Action<object> set, Action invoke, NumberOptions number = default, Func<IReadOnlyList<UIText>> options = null, OrderAttribute order = null)
	{
		return Add(new SettingEntry(this, kind, label, help, null, null, get, set, invoke, number, false, options, order));
	}

	public ISettingHandle AddButton(UIText label, Action onClick, UIText help = default)
	{
		return AddManual(SettingKind.Action, label, help, null, null, onClick);
	}

	public ISettingHandle AddButton(UIText label, Action onClick, int order, UIText help = default)
	{
		return AddManual(SettingKind.Action, label, help, null, null, onClick, order: new OrderAttribute(order));
	}

	public ISettingHandle AddToggle(UIText label, Func<bool> get, Action<bool> set, UIText help = default)
	{
		return AddManual(SettingKind.Toggle, label, help, () => get(), v => set((bool)v), null);
	}

	public ISettingHandle AddNumber(UIText label, Func<double> get, Action<double> set, NumberOptions options, UIText help = default)
	{
		return AddManual(SettingKind.Number, label, help, () => get(), v => set(Convert.ToDouble(v)), null, options);
	}

	public ISettingHandle AddChoice(UIText label, Func<IReadOnlyList<UIText>> options, Func<int> get, Action<int> set, UIText help = default)
	{
		return AddManual(SettingKind.Choice, label, help, () => get(), v => set(Convert.ToInt32(v)), null, default, options);
	}

	public ISettingHandle AddText(UIText label, Func<string> get, Action<string> set, UIText help = default)
	{
		return AddManual(SettingKind.Text, label, help, () => get(), v => set((string)v), null);
	}
}
