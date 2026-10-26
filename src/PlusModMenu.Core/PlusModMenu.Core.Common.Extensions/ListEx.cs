using System;
using System.Collections.Generic;

namespace PlusModMenu.Core.Common.Extensions;

public static class ListEx
{
	public static bool TryGetValue<T>(this List<T> list, int index, out T? result)
	{
		try
		{
			result = list[index];
			return true;
		}
		catch (ArgumentOutOfRangeException)
		{
			result = default;
			return false;
		}
	}
}
