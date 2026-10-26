using ModMenu.Settings;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Clipboard.Rows;

internal sealed class Vector2Row : SettingRow
{
	private InlineEditor _x;

	private InlineEditor _y;

	protected override float ControlAreaWidth => 200f;

	protected override void Build()
	{
		_x = Component(-50f, 0);
		_y = Component(50f, 1);
	}

	private InlineEditor Component(float centre, int axis)
	{
		HeaderBar.Arrow("Less" + axis, Control, new Vector2(centre - 34f, 0f), left: true, () => Step(axis, -1));
		HeaderBar.Arrow("More" + axis, Control, new Vector2(centre + 34f, 0f), left: false, () => Step(axis, 1));
		return new InlineEditor(this, Control, new Vector2(centre, 0f), 36f, 60f, AssetNames.Font12, () => InlineEditor.FormatExact(Value[axis]), text => Apply(axis, text));
	}

	private Vector2 Value => Entry.GetValue() is Vector2 v ? v : Vector2.zero;

	private void Step(int axis, int direction)
	{
		if (!Entry.Available)
		{
			return;
		}
		var value = Value;
		value[axis] = (float)StepRule.Apply(value[axis], StepRule.For(integer: false, value[axis], Entry.Number.Step), direction, null, null);
		Entry.SetValue(value);
	}

	private bool Apply(int axis, string text)
	{
		if (!Entry.Available || !NumberParser.TryParse(text, integer: false, null, null, out var parsed))
		{
			return false;
		}
		var value = Value;
		value[axis] = (float)parsed;
		Entry.SetValue(value);
		return true;
	}

	public override void ReadValue()
	{
		var value = Value;
		_x.Display.text = InlineEditor.Format(value.x);
		_y.Display.text = InlineEditor.Format(value.y);
	}
}
