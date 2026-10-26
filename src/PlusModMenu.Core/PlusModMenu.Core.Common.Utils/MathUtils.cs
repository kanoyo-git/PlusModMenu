using System;

namespace PlusModMenu.Core.Common.Utils;

public class MathUtils
{
	public static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
	{
		if (val.CompareTo(min) < 0)
		{
			return min;
		}
		if (val.CompareTo(max) > 0)
		{
			return max;
		}
		return val;
	}

	public static bool IsInRange<T>(T val, T min, T max) where T : IComparable<T>
	{
		if (val.CompareTo(min) >= 0)
		{
			return val.CompareTo(max) <= 0;
		}
		return false;
	}
}
