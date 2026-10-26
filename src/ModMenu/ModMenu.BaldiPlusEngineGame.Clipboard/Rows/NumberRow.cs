using System;
using System.Globalization;
using ModMenu.Settings;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Clipboard.Rows;

internal sealed class NumberRow : SettingRow
{
	private InlineEditor _editor;

	public InlineEditor Editor => _editor;

	protected override void Build()
	{
		HeaderBar.Arrow("Less", Control, new Vector2(-60f, 0f), left: true, () => Step(-1));
		HeaderBar.Arrow("More", Control, new Vector2(60f, 0f), left: false, () => Step(1));
		_editor = new InlineEditor(this, Control, Vector2.zero, 84f, 84f, AssetNames.Font18, () => InlineEditor.FormatExact(Entry.GetValue()), Apply);
	}

	private void Step(int direction)
	{
		if (!Entry.Available)
		{
			return;
		}
		var current = Numbers.ToDouble(Entry.GetValue());
		var step = StepRule.For(Entry.Number.Integer, current, Entry.Number.Step);
		Entry.SetValue(Numbers.Like(StepRule.Apply(current, step, direction, Entry.Number.Min, Entry.Number.Max), Entry.GetValue()));
	}

	public bool Apply(string text)
	{
		if (!Entry.Available || !NumberParser.TryParse(text, Entry.Number.Integer, Entry.Number.Min, Entry.Number.Max, out var value))
		{
			return false;
		}
		Entry.SetValue(Numbers.Like(value, Entry.GetValue()));
		return true;
	}

	public override void ReadValue()
	{
		_editor.Display.text = InlineEditor.Format(Entry.GetValue());
	}
}

internal static class Numbers
{
	public static double ToDouble(object value)
	{
		return value is IConvertible convertible ? convertible.ToDouble(CultureInfo.InvariantCulture) : 0.0;
	}

	public static object Like(double value, object current)
	{
		return current switch
		{
			int _ => (object)(int)Math.Round(Math.Max(int.MinValue, Math.Min(int.MaxValue, value))),
			float _ => (float)value,
			_ => value,
		};
	}
}
