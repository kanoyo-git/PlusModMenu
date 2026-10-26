using System;
using System.Collections.Generic;

namespace PlusModMenu.Api.Collectors;

public class InstanceCollector<TItem>
{
	private readonly List<TItem> _collection = new List<TItem>();

	public IReadOnlyList<TItem> Collection => _collection.AsReadOnly();

	public event Action<TItem>? ItemAdded;

	public event Action<TItem>? ItemRemoved;

	public void Add(TItem item)
	{
		if (!_collection.Contains(item))
		{
			_collection.Add(item);
			ItemAdded?.Invoke(item);
		}
	}

	public void Remove(TItem item)
	{
		if (_collection.Contains(item))
		{
			_collection.Remove(item);
			ItemRemoved?.Invoke(item);
		}
	}

	public void Clear()
	{
		_collection.Clear();
	}
}
