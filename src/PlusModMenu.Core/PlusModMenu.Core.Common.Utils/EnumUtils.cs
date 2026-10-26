using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PlusModMenu.Core.Common.Utils;

public static class EnumUtils
{
	private static readonly Dictionary<Type, Enum[]> _enumValueCache = new Dictionary<Type, Enum[]>();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Enum Next(Type enumType, Enum enumValue)
	{
		Enum[] values = GetValues(enumType);
		int num = Array.IndexOf(values, enumValue) + 1;
		if (values.Length != num)
		{
			return values[num];
		}
		return values[0];
	}

	public static T Next<T>(T enumValue) where T : Enum
	{
		return (T)Next(enumValue.GetType(), enumValue);
	}

	public static T[] GetValues<T>() where T : Enum
	{
		return GetValues(typeof(T)).Cast<T>().ToArray();
	}

	public static Enum[] GetValues(Type enumType)
	{
		if (!_enumValueCache.TryGetValue(enumType, out Enum[] value))
		{
			value = Enum.GetValues(enumType).Cast<Enum>().ToArray();
			_enumValueCache.Add(enumType, value);
		}
		return value;
	}
}
