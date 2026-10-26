using System;
using System.Collections.Generic;
using System.Linq;

namespace ModMenu.Settings;

public static class ConfigText
{
	private const char Separator = ';';

	public static Dictionary<string, string> ParseMap(string? text)
	{
		var map = new Dictionary<string, string>();
		foreach (string entry in Split(text))
		{
			int equals = entry.LastIndexOf('=');
			if (equals <= 0)
			{
				continue;
			}
			string id = entry.Substring(0, equals).Trim();
			string value = entry.Substring(equals + 1).Trim();
			if (id.Length > 0 && value.Length > 0)
			{
				map[id] = value;
			}
		}
		return map;
	}

	public static string FormatMap(IEnumerable<KeyValuePair<string, string>> map)
	{
		return string.Join(Separator + " ", map.OrderBy(x => x.Key, StringComparer.Ordinal).Select(x => x.Key + " = " + x.Value));
	}

	public static List<string> ParseList(string? text)
	{
		return Split(text).Distinct().ToList();
	}

	public static string FormatList(IEnumerable<string> list)
	{
		return string.Join(Separator + " ", list);
	}

	private static IEnumerable<string> Split(string? text)
	{
		return (text ?? string.Empty).Split(Separator).Select(x => x.Trim()).Where(x => x.Length > 0);
	}
}
