using System;
using System.Collections;
using System.Collections.Generic;
using PlusModMenu.Core.Common.Extensions;

namespace ModMenu.Common;

public class CallbackList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	private readonly List<T> _list = new List<T>();

	public int Count => _list.Count;

	public bool IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

	public T this[int index]
	{
		get
		{
			return _list[index];
		}
		set
		{
			_list[index] = value;
			ItemAdded?.Invoke(value);
		}
	}

	public event Action<T>? ItemAdded;

	public event Action<T>? ItemRemoved;

	public IEnumerator<T> GetEnumerator()
	{
		return _list.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)_list).GetEnumerator();
	}

	public void Add(T item)
	{
		_list.Add(item);
		ItemAdded?.Invoke(item);
	}

	public void Insert(int index, T item)
	{
		_list.Insert(index, item);
		ItemAdded?.Invoke(item);
	}

	public bool Remove(T item)
	{
		if (!_list.Remove(item))
		{
			return false;
		}
		ItemRemoved?.Invoke(item);
		return true;
	}

	public void RemoveAt(int index)
	{
		_list.TryGetValue(index, out T result);
		_list.RemoveAt(index);
		ItemRemoved?.Invoke(result);
	}

	public void Clear()
	{
		_list.Clear();
	}

	public bool Contains(T item)
	{
		return ((ICollection<T>)_list).Contains(item);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		((ICollection<T>)_list).CopyTo(array, arrayIndex);
	}

	public int IndexOf(T item)
	{
		return _list.IndexOf(item);
	}
}
