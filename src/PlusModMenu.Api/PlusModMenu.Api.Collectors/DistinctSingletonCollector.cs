using System;
using System.Collections.Generic;
using System.Linq;

namespace PlusModMenu.Api.Collectors;

public class DistinctSingletonCollector<TItem, TEqualityComparer> where TEqualityComparer : IEqualityComparer<TItem>, new()
{
	private static readonly List<TItem> _collection = new List<TItem>();

	private static readonly TEqualityComparer _equalityComparer = new TEqualityComparer();

	public static IReadOnlyList<TItem> Collection => _collection.AsReadOnly();

	public static event Action<TItem>? ItemAdded;

	public static event Action<TItem>? ItemRemoved;

	public static void Add(TItem item)
	{
		if (!_collection.Contains(item, _equalityComparer))
		{
			_collection.Add(item);
			ItemAdded?.Invoke(item);
		}
	}

	public static void Remove(TItem item)
	{
		if (_collection.Contains(item, _equalityComparer))
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
