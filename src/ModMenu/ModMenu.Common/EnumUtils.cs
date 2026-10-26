using System;
using System.Collections.Generic;
using System.Reflection;
using PlusModMenu.Core.Common.Extensions;
using PlusModMenu.Api.Attributes;

namespace ModMenu.Common;

public static class EnumUtils
{
	private static readonly Dictionary<Enum, string> _enumLabelCache = new Dictionary<Enum, string>();

	public static string GetLabel(Enum enumValue)
	{
		if (_enumLabelCache.TryGetValue(enumValue, out string value))
		{
			return value;
		}
		Type type = enumValue.GetType();
		foreach (Enum value2 in Enum.GetValues(type))
		{
			string text = value2.ToString();
			value = type.GetField(text, BindingFlags.Static | BindingFlags.Public)?.GetAttribute<LabelAttribute>()?.Text ?? text;
			_enumLabelCache.Add(value2, value);
		}
		return value ?? string.Empty;
	}
}
