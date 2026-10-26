using System.Collections.Generic;
using PlusModMenu.Api.Settings;

namespace ModMenu.Settings;

public static class SettingKeep
{
	public static bool CanKeep(SettingEntry entry)
	{
		return entry.Kind == SettingKind.Toggle && entry.Setter != null;
	}

	public static bool Flip(ICollection<string> kept, SettingEntry entry)
	{
		var id = SettingBinds.Id(entry);
		if (kept.Remove(id))
		{
			return false;
		}
		if (!(entry.GetValue() is true) && entry.Available)
		{
			entry.SetValue(true);
		}
		kept.Add(id);
		return true;
	}

	public static int Apply(IEnumerable<SettingEntry> entries, ICollection<string> kept, ISet<string> applied)
	{
		var count = 0;
		foreach (var entry in entries)
		{
			if (entry.IsRemoved || !CanKeep(entry) || !entry.Available)
			{
				continue;
			}
			var id = SettingBinds.Id(entry);
			if (!kept.Contains(id) || !applied.Add(id))
			{
				continue;
			}
			if (!(entry.GetValue() is true))
			{
				entry.SetValue(true);
			}
			count++;
		}
		return count;
	}
}
