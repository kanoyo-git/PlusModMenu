using System;
using PlusModMenu.Api.Settings;

namespace ModMenu.Settings;

public static class SettingBinds
{
	public static bool CanBind(SettingEntry entry)
	{
		return entry.Kind switch
		{
			SettingKind.Toggle => entry.Setter != null,
			SettingKind.Choice => entry.Setter != null && entry.Options != null,
			SettingKind.Action => entry.Invoke != null,
			_ => false,
		};
	}

	public static string Id(SettingEntry entry)
	{
		var group = entry.Group;
		var page = group.Page;
		return Part(page.Section.StaticType, page.Section.Label) + "/" + Part(page.StaticType, page.Label) + "/" + Part(group.StaticType, group.Label) + "/" + (entry.MemberName ?? entry.Label.Value);
	}

	private static string Part(Type type, UIText label)
	{
		return type?.FullName + ":" + label.Value;
	}

	public static bool Activate(SettingEntry entry)
	{
		if (entry.IsRemoved || !entry.Available || !CanBind(entry))
		{
			return false;
		}
		switch (entry.Kind)
		{
		case SettingKind.Toggle:
			entry.SetValue(!(entry.GetValue() is true));
			return true;
		case SettingKind.Choice:
			var count = entry.Options().Count;
			if (count < 2)
			{
				return false;
			}
			var index = entry.GetValue() is int i ? i : 0;
			entry.SetValue((index + 1) % count);
			return true;
		default:
			entry.Invoke();
			return true;
		}
	}
}
