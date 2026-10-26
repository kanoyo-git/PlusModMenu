using System.Collections.Generic;

namespace PlusModMenu.Core.Common.Extensions;

public static class DictionaryEx
{
	public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, TValue value)
	{
		if (@this.ContainsKey(key))
		{
			return false;
		}
		@this.Add(key, value);
		return true;
	}

	public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> @this, IDictionary<TKey, TValue> dictionary)
	{
		foreach (KeyValuePair<TKey, TValue> item in dictionary)
		{
			@this.Add(item.Key, item.Value);
		}
	}

	public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, TValue @default)
	{
		if (!@this.ContainsKey(key))
		{
			return @default;
		}
		return @this[key];
	}
}
