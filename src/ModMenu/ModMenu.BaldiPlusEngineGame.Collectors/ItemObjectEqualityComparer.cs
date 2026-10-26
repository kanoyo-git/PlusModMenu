using System;
using System.Collections.Generic;

namespace ModMenu.BaldiPlusEngineGame.Collectors;

public class ItemObjectEqualityComparer : IEqualityComparer<ItemObject>
{
	public bool Equals(ItemObject x, ItemObject y)
	{
		if (!string.Equals(x.nameKey, y.nameKey, StringComparison.Ordinal))
		{
			return string.Equals(x.descKey, y.descKey, StringComparison.Ordinal);
		}
		return true;
	}

	public int GetHashCode(ItemObject obj)
	{
		return 1;
	}
}
