using System;

namespace ModMenu.Settings;

public static class SettingKinds
{
	public const double MaxRangeSpan = 1000;

	public static SettingKind? For(Type valueType, double? rangeMin, double? rangeMax, bool forceRange)
	{
		if (valueType == null || valueType == typeof(void))
		{
			return SettingKind.Action;
		}
		if (valueType == typeof(bool))
		{
			return SettingKind.Toggle;
		}
		if (valueType == typeof(string))
		{
			return SettingKind.Text;
		}
		if (valueType.IsEnum)
		{
			return SettingKind.Choice;
		}
		if (valueType == typeof(int) || valueType == typeof(float) || valueType == typeof(double))
		{
			var finite = rangeMin is double min && rangeMax is double max && !double.IsInfinity(min) && !double.IsInfinity(max) && max > min;
			return finite && rangeMax.Value - rangeMin.Value <= MaxRangeSpan ? SettingKind.Range : SettingKind.Number;
		}
		switch (valueType.FullName)
		{
		case "UnityEngine.Color":
		case "UnityEngine.Color32":
			return SettingKind.Color;
		case "UnityEngine.Vector2":
			return SettingKind.Vector2;
		default:
			return null;
		}
	}
}
