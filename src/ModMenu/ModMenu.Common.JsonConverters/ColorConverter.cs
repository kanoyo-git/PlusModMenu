using System;
using PlusModMenu.Core.Unity.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace ModMenu.Common.JsonConverters;

internal class ColorConverter : JsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		if (!(objectType == typeof(Color)))
		{
			return objectType == typeof(Color32);
		}
		return true;
	}

	public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
	{
		return ColorUtils.ColorFromHTMLString((string)reader.Value);
	}

	public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
	{
		writer.WriteValue(ColorUtility.ToHtmlStringRGBA((Color)value));
	}
}
