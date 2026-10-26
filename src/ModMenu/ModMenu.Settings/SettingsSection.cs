using System;
using System.Collections.Generic;
using PlusModMenu.Api.Settings;

namespace ModMenu.Settings;

public sealed class SettingsSection
{
	internal readonly List<SettingsPage> PageList = new List<SettingsPage>();

	public Type StaticType { get; }

	public UIText Label { get; }

	public OrderAttribute Order { get; }

	internal SettingsRegistry Registry { get; }

	public IReadOnlyList<SettingsPage> Pages => OrderRules.Sort(PageList, p => p.Order);

	internal SettingsSection(SettingsRegistry registry, Type staticType, UIText label, OrderAttribute order)
	{
		Registry = registry;
		StaticType = staticType;
		Label = label;
		Order = order;
	}
}
