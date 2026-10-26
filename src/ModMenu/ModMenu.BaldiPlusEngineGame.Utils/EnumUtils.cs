using System;
using System.Collections.Generic;
using System.Reflection;
using PlusModMenu.Core.Common.Extensions;
using PlusModMenu.Api.Attributes;

namespace ModMenu.BaldiPlusEngineGame.Utils;

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
			LabelAttribute labelAttribute = type.GetField(text, BindingFlags.Static | BindingFlags.Public)?.GetAttribute<LabelAttribute>();
			if (labelAttribute != null)
			{
				LabelAttribute labelAttribute2 = labelAttribute;
				_enumLabelCache[value2] = labelAttribute2.Text;
			}
			else
			{
				_enumLabelCache[value2] = text;
			}
		}
		return _enumLabelCache[enumValue];
	}
}
