using System;

namespace ModMenu.Settings;

public static class RangeBars
{
	public const int Count = 10;

	public static int Filled(double value, double min, double max)
	{
		if (!(max > min))
		{
			return 0;
		}
		var share = (value - min) / (max - min);
		return (int)Math.Round(Math.Max(0, Math.Min(1, share)) * Count, MidpointRounding.AwayFromZero);
	}

	public static double ValueForBar(int bar, double min, double max)
	{
		return min + (max - min) * (Math.Max(0, Math.Min(Count - 1, bar)) + 1) / Count;
	}
}
