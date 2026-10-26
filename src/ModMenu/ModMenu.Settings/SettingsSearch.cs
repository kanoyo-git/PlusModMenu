using System;
using System.Collections.Generic;
using System.Linq;
using PlusModMenu.Api.Settings;

namespace ModMenu.Settings;

public static class SettingsSearch
{
	private static readonly char[] Whitespace = { ' ', '\t', '\n', '\r', ' ' };

	private const string English = "`qwertyuiop[]asdfghjkl;'zxcvbnm,.";

	private const string Russian = "ёйцукенгшщзхъфывапролджэячсмитьбю";

	public static List<SettingEntry> Find(IEnumerable<SettingEntry> entries, string query, Func<UIText, string> resolve, params Func<UIText, string>[] alsoIn)
	{
		var result = new List<SettingEntry>();
		var words = (query ?? string.Empty).Split(Whitespace, StringSplitOptions.RemoveEmptyEntries).Select(w => w.ToLowerInvariant()).ToArray();
		if (words.Length == 0)
		{
			return result;
		}
		var languages = new[] { resolve }.Concat(alsoIn ?? Array.Empty<Func<UIText, string>>()).ToArray();
		var variants = words.Select(w => new[] { w, OtherLayout(w) }.Distinct().ToArray()).ToArray();
		foreach (var entry in entries)
		{
			if (!entry.Visible || entry.IsRemoved || entry.Group.IsRemoved)
			{
				continue;
			}
			var text = string.Join(" ", languages.Select(language => language(entry.Label) + " " + language(entry.Help))).ToLowerInvariant();
			if (variants.All(word => word.Any(variant => text.IndexOf(variant, StringComparison.Ordinal) >= 0)))
			{
				result.Add(entry);
			}
		}
		return result;
	}

	public static string OtherLayout(string word)
	{
		var chars = word.ToCharArray();
		for (var i = 0; i < chars.Length; i++)
		{
			var e = English.IndexOf(chars[i]);
			var r = Russian.IndexOf(chars[i]);
			if (e >= 0) chars[i] = Russian[e];
			else if (r >= 0) chars[i] = English[r];
		}
		return new string(chars);
	}
}
