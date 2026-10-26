using System;
using ModMenu.Settings;
using TMPro;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Clipboard;

internal static class ColorPicker
{
	private const float Cell = 32f;

	private const float CellHeight = 16f;

	private const float Gap = 4f;

	private const int Columns = 4;

	public static void Show(ClipboardMenu menu, RectTransform anchor, Color32 current, Action<Color32> set)
	{
		var rows = (ColorPalette.Colors.Length + Columns - 1) / Columns;
		var width = Columns * Cell + (Columns - 1) * Gap + 16f;
		var height = rows * (CellHeight + Gap) + 30f + 12f;
		var popup = menu.ShowPopup("ColorPicker");
		var frame = UiFactory.Frame("Frame", popup);
		frame.raycastTarget = true;
		var position = menu.PopupPosition(anchor) - new Vector2(0f, anchor.rect.height / 2f + height / 2f + 2f);
		frame.rectTransform.Place(Clamp(menu, position, width, height), new Vector2(width, height));
		var color = current;
		for (var i = 0; i < ColorPalette.Colors.Length; i++)
		{
			var (r, g, b) = ColorPalette.Colors[i];
			var x = 8f + i % Columns * (Cell + Gap);
			var y = 8f + i / Columns * (CellHeight + Gap);
			UiFactory.Fill("Border" + i, frame.rectTransform, UiFactory.Grey).rectTransform.PlaceTopLeft(x - 1f, y - 1f, Cell + 2f, CellHeight + 2f);
			var swatch = UiFactory.Fill("Swatch" + i, frame.rectTransform, new Color32(r, g, b, byte.MaxValue));
			swatch.rectTransform.PlaceTopLeft(x, y, Cell, CellHeight);
			UiFactory.Clickable(swatch, () =>
			{
				color = new Color32(r, g, b, byte.MaxValue);
				set(color);
				menu.ClosePopup();
			});
		}
		var fieldY = 8f + rows * (CellHeight + Gap) + 2f;
		var fieldWidth = (width - 16f - 2f * Gap) / 3f;
		for (var channel = 0; channel < 3; channel++)
		{
			var index = channel;
			var label = UiFactory.Text("Label" + channel, frame.rectTransform, AssetNames.Font12, "RGB".Substring(channel, 1));
			label.rectTransform.PlaceTopLeft(8f + channel * (fieldWidth + Gap), fieldY, 10f, 22f);
			var field = UiFactory.Input("Channel" + channel, frame.rectTransform, AssetNames.Font12, fieldWidth - 10f, 20f);
			field.GetComponent<RectTransform>().PlaceTopLeft(18f + channel * (fieldWidth + Gap), fieldY + 1f, fieldWidth - 10f, 20f);
			field.characterLimit = 3;
			field.SetTextWithoutNotify(Channel(color, index).ToString());
			field.onEndEdit.AddListener(text =>
			{
				if (field.wasCanceled)
				{
					return;
				}
				if (NumberParser.TryParse(text, integer: true, 0, 255, out var value))
				{
					field.textComponent.color = UiFactory.Ink;
					color = WithChannel(color, index, (byte)value);
					set(color);
				}
				else
				{
					field.textComponent.color = UiFactory.Red;
				}
			});
		}
	}

	private static Vector2 Clamp(ClipboardMenu menu, Vector2 position, float width, float height)
	{
		var halfWidth = menu.Layout.CanvasWidth / 2f - width / 2f - 4f;
		var halfHeight = ClipboardLayout.CanvasHeight / 2f - height / 2f - 4f;
		return new Vector2(Mathf.Clamp(position.x, 0f - halfWidth, halfWidth), Mathf.Clamp(position.y, 0f - halfHeight, halfHeight));
	}

	private static byte Channel(Color32 color, int index)
	{
		return index switch
		{
			0 => color.r,
			1 => color.g,
			_ => color.b,
		};
	}

	private static Color32 WithChannel(Color32 color, int index, byte value)
	{
		switch (index)
		{
		case 0:
			color.r = value;
			break;
		case 1:
			color.g = value;
			break;
		default:
			color.b = value;
			break;
		}
		return color;
	}
}
