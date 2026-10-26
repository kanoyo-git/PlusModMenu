using System;
using System.Collections.Generic;
using System.Linq;

namespace PlusModMenu.Core.Common.Extensions;

public static class EnumerableEx
{
	public static int IndexOf<T>(this IReadOnlyList<T> list, T item)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].Equals(item))
			{
				return i;
			}
		}
		return -1;
	}

	public static IEnumerable<T> Concat<T>(params IEnumerable<T>[] enumerables)
	{
		if (enumerables.Any((IEnumerable<T> x) => x == null))
		{
			throw new ArgumentNullException("enumerables", "One or more enumerables is null.");
		}
		IEnumerable<T> enumerable = Enumerable.Empty<T>();
		foreach (IEnumerable<T> second in enumerables)
		{
			enumerable = enumerable.Concat(second);
		}
		return enumerable;
	}
}
