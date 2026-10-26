using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlusModMenu.Api.Collectors;

public class MonoBehaviourCollector<TItem, TCollectorItem> where TItem : MonoBehaviour where TCollectorItem : MonoBehaviourCollectorItem
{
	private static readonly List<TCollectorItem> _collection = new List<TCollectorItem>();

	public static IReadOnlyList<TCollectorItem> Collection => _collection.AsReadOnly();

	public static event Action<TCollectorItem>? ItemAdded;

	public static event Action<TCollectorItem>? ItemRemoved;

	public static void Add(TItem item)
	{
		if (!(item.gameObject.GetComponent<TCollectorItem>() != null))
		{
			item.gameObject.AddComponent<TCollectorItem>().Create(item);
		}
	}

	internal static void InternalAdd(TCollectorItem item)
	{
		_collection.Add(item);
		ItemAdded?.Invoke(item);
	}

	internal static void InternalRemove(TCollectorItem item)
	{
		_collection.Remove(item);
		ItemRemoved?.Invoke(item);
	}
}
