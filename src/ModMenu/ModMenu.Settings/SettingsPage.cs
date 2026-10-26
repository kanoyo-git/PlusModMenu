using System;
using System.Collections.Generic;
using System.Linq;
using PlusModMenu.Api.Settings;

namespace ModMenu.Settings;

public sealed class SettingsPage : ISettingsPage
{
	private readonly List<SettingsGroup> _groups = new List<SettingsGroup>();

	public Type StaticType { get; }

	public UIText Label { get; }

	public SettingsSection Section { get; }

	public OrderAttribute Order { get; }

	internal SettingsRegistry Registry => Section.Registry;

	public IReadOnlyList<SettingsGroup> Groups => OrderRules.Sort(_groups, g => g.Order);

	public IEnumerable<SettingEntry> Entries => Groups.SelectMany(g => g.Entries);

	internal SettingsPage(SettingsSection section, Type staticType, UIText label, OrderAttribute order)
	{
		Section = section;
		StaticType = staticType;
		Label = label;
		Order = order;
	}

	internal SettingsGroup AddGroup(Type staticType, UIText label, OrderAttribute order)
	{
		var group = new SettingsGroup(this, staticType, label, order);
		_groups.Add(group);
		return group;
	}

	public ISettingsGroup AddGroup(UIText label)
	{
		var group = AddGroup(null, label, null);
		Registry.RaisePageChanged(this);
		return group;
	}

	public ISettingsGroup GetGroup(Type staticGroupType)
	{
		return _groups.FirstOrDefault(g => g.StaticType == staticGroupType && !g.IsRemoved)
			?? throw new InvalidOperationException("Group " + staticGroupType?.FullName + " is not on page " + StaticType?.FullName + ".");
	}

	internal void RemoveGroup(SettingsGroup group)
	{
		if (!group.IsRemoved && _groups.Remove(group))
		{
			group.IsRemoved = true;
			foreach (var entry in group.AllEntries)
			{
				entry.IsRemoved = true;
				Registry.OnEntryRemoved(entry);
			}
			Registry.RaisePageChanged(this);
		}
	}

	public void ResetToDefaults()
	{
		foreach (var entry in _groups.SelectMany(g => g.AllEntries).Where(e => e.Available).ToList())
		{
			entry.ResetToDefault();
			entry.Lock?.ResetToDefault();
		}
	}
}
