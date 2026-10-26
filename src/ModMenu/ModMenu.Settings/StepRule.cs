using System;

namespace ModMenu.Settings;

public static class StepRule
{
	public static double For(bool integer, double current, double? explicitStep)
	{
		if (explicitStep is double step && step > 0)
		{
			return step;
		}
		if (integer)
		{
			return 1;
		}
		return Math.Abs(current) < 10 ? 0.1 : 1;
	}

	public static double Apply(double current, double step, int direction, double? min, double? max)
	{
		var next = Math.Round(current + step * Math.Sign(direction), 6);
		if (min is double lo && next < lo)
		{
			next = lo;
		}
		if (max is double hi && next > hi)
		{
			next = hi;
		}
		return next;
	}
}
