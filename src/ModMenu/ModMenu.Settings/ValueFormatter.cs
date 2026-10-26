using System;
using System.Globalization;
using System.Reflection;

namespace ModMenu.Settings;

public static class ValueFormatter
{
	public static string Format(object value)
	{
		switch (value)
		{
		case null:
			return "—";
		case bool b:
			return b ? "On" : "Off";
		case float f:
			return f.ToString("0.##", CultureInfo.InvariantCulture);
		case double d:
			return d.ToString("0.##", CultureInfo.InvariantCulture);
		case IFormattable formattable when !(value is Enum):
			return formattable.ToString(null, CultureInfo.InvariantCulture);
		}
		switch (value.GetType().FullName)
		{
		case "UnityEngine.Color":
			return Hex(ToByte(Field<float>(value, "r")), ToByte(Field<float>(value, "g")), ToByte(Field<float>(value, "b")));
		case "UnityEngine.Color32":
			return Hex(Field<byte>(value, "r"), Field<byte>(value, "g"), Field<byte>(value, "b"));
		default:
			return value.ToString();
		}
	}

	public static string FormatExact(object value)
	{
		switch (value)
		{
		case float f:
			return f.ToString("R", CultureInfo.InvariantCulture);
		case double d:
			return d.ToString("R", CultureInfo.InvariantCulture);
		default:
			return Format(value);
		}
	}

	private static T Field<T>(object value, string name)
	{
		return (T)value.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public).GetValue(value);
	}

	private static byte ToByte(float channel)
	{
		return (byte)Math.Round(Math.Max(0f, Math.Min(1f, channel)) * 255f);
	}

	private static string Hex(byte r, byte g, byte b)
	{
		return "#" + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
	}
}
