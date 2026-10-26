using System;
using System.Collections;
using System.Collections.Generic;

namespace ModMenu.Common;

public class CallbackDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
{
	private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

	public int Count => _dictionary.Count;

	public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).IsReadOnly;

	public TValue this[TKey key]
	{
		get
		{
			return _dictionary[key];
		}
		set
		{
			if (!ContainsKey(key))
			{
				Add(key, value);
			}
			else
			{
				_dictionary[key] = value;
			}
		}
	}

	public ICollection<TKey> Keys => _dictionary.Keys;

	public ICollection<TValue> Values => _dictionary.Values;

	public event Action<TKey, TValue>? ItemAdded;

	public event Action<TKey, TValue>? ItemRemoved;

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		return _dictionary.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)_dictionary).GetEnumerator();
	}

	public void Add(KeyValuePair<TKey, TValue> item)
	{
		Add(item.Key, item.Value);
	}

	public void Clear()
	{
		_dictionary.Clear();
	}

	public bool Contains(KeyValuePair<TKey, TValue> item)
	{
		return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item);
	}

	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
	}

	public bool Remove(KeyValuePair<TKey, TValue> item)
	{
		return Remove(item.Key);
	}

	public bool ContainsKey(TKey key)
	{
		return _dictionary.ContainsKey(key);
	}

	public void Add(TKey key, TValue value)
	{
		_dictionary.Add(key, value);
		ItemAdded?.Invoke(key, value);
	}

	public bool Remove(TKey key)
	{
		_dictionary.TryGetValue(key, out var value);
		if (_dictionary.Remove(key))
		{
			ItemRemoved?.Invoke(key, value);
			return true;
		}
		return false;
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		return _dictionary.TryGetValue(key, out value);
	}
}
