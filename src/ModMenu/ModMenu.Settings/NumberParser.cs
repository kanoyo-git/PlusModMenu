using System.Globalization;

namespace ModMenu.Settings;

public static class NumberParser
{
	public static bool TryParse(string text, bool integer, double? min, double? max, out double value)
	{
		value = 0;
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}
		if (!double.TryParse(text.Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
			|| double.IsNaN(parsed) || double.IsInfinity(parsed))
		{
			return false;
		}
		if (integer && (parsed != System.Math.Floor(parsed) || parsed < int.MinValue || parsed > int.MaxValue))
		{
			return false;
		}
		if (min is double lo && parsed < lo || max is double hi && parsed > hi)
		{
			return false;
		}
		value = parsed;
		return true;
	}
}
