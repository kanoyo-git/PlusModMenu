using System;
using System.Collections.Generic;
using System.Linq;

namespace ModMenu.Settings;

public static class OrderRules
{
	public static List<T> Sort<T>(IEnumerable<T> items, Func<T, OrderAttribute> order)
	{
		var indexed = items.Select((item, index) => (item, index, order: order(item))).ToList();
		return indexed.Where(x => x.order?.Mode == OrderAttribute.OrderMode.BeforeUnordered).OrderBy(x => x.order.Order).ThenBy(x => x.index)
			.Concat(indexed.Where(x => x.order == null))
			.Concat(indexed.Where(x => x.order?.Mode == OrderAttribute.OrderMode.AfterUnordered).OrderBy(x => x.order.Order).ThenBy(x => x.index))
			.Select(x => x.item)
			.ToList();
	}
}
