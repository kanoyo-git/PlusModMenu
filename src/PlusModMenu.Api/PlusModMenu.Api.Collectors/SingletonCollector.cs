using System;
using System.Collections.Generic;

namespace PlusModMenu.Api.Collectors;

public class SingletonCollector<TItem>
{
	private static readonly List<TItem> _collection = new List<TItem>();

	public static IReadOnlyList<TItem> Collection => _collection.AsReadOnly();

	public static event Action<TItem>? ItemAdded;

	public static event Action<TItem>? ItemRemoved;

	public static void Add(TItem item)
	{
		if (!_collection.Contains(item))
		{
			_collection.Add(item);
			ItemAdded?.Invoke(item);
		}
	}

	public static void Remove(TItem item)
	{
		if (_collection.Contains(item))
		{
			_collection.Remove(item);
			ItemRemoved?.Invoke(item);
		}
	}

	public static void Clear()
	{
		_collection.Clear();
	}
}
