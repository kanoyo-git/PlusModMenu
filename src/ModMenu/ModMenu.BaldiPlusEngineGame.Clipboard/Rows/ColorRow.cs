using ModMenu.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace ModMenu.BaldiPlusEngineGame.Clipboard.Rows;

internal sealed class ColorRow : SettingRow
{
	private Image _sample;

	private RectTransform _frame;

	protected override void Build()
	{
		var frame = UiFactory.Frame("Sample", Control);
		_frame = frame.rectTransform;
		_frame.Place(Vector2.zero, new Vector2(40f, 22f));
		_sample = UiFactory.Fill("Colour", _frame, Color.white);
		_sample.rectTransform.Stretch(3f, 3f, 3f, 3f);
		UiFactory.Clickable(frame, Open);
	}

	public void Open()
	{
		if (Entry.Available && ClipboardMenu.Instance != null)
		{
			ColorPicker.Show(ClipboardMenu.Instance, _frame, Current, SetColor);
		}
	}

	private Color32 Current => Entry.GetValue() switch
	{
		Color32 c32 => c32,
		Color c => (Color32)c,
		_ => new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue),
	};

	private void SetColor(Color32 color)
	{
		var current = Current;
		if (color.r == current.r && color.g == current.g && color.b == current.b)
		{
			return;
		}
		switch (Entry.GetValue())
		{
		case Color32 old:
			Entry.SetValue(new Color32(color.r, color.g, color.b, old.a));
			break;
		case Color old:
			Color value = (Color)color;
			value.a = old.a;
			Entry.SetValue(value);
			break;
		}
	}

	public override void ReadValue()
	{
		var color = Current;
		_sample.color = new Color32(color.r, color.g, color.b, byte.MaxValue);
	}
}
