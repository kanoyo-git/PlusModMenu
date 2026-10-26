using ModMenu.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModMenu.BaldiPlusEngineGame.Clipboard.Rows;

internal sealed class RangeRow : SettingRow
{
	private readonly Image[] _bars = new Image[RangeBars.Count];

	private InlineEditor _editor;

	protected override float ControlAreaWidth => 200f;

	protected override void Build()
	{
		_editor = new InlineEditor(this, Control, new Vector2(-86f, 0f), 30f, 56f, AssetNames.Font12, () => InlineEditor.FormatExact(Entry.GetValue()), Apply, new Vector2(-70f, 0f));
		_editor.Display.alignment = TextAlignmentOptions.Right;
		HeaderBar.Arrow("Less", Control, new Vector2(-52f, 0f), left: true, () => Step(-1));
		for (var i = 0; i < _bars.Length; i++)
		{
			var bar = i;
			_bars[i] = UiFactory.Image("Bar" + i, Control, GameAssets.Sprite(AssetNames.BarEmpty));
			_bars[i].rectTransform.Place(new Vector2(-30f + 10f * i, 0f), new Vector2(8f, 32f));
			UiFactory.Clickable(_bars[i], () => SetBar(bar));
		}
		HeaderBar.Arrow("More", Control, new Vector2(82f, 0f), left: false, () => Step(1));
	}

	private double Min => Entry.Number.Min ?? 0.0;

	private double Max => Entry.Number.Max ?? 1.0;

	private void Step(int direction)
	{
		if (!Entry.Available)
		{
			return;
		}
		var current = Numbers.ToDouble(Entry.GetValue());
		var step = Entry.Number.Step ?? (Entry.Number.Integer ? System.Math.Max(1.0, System.Math.Round((Max - Min) / RangeBars.Count)) : (Max - Min) / RangeBars.Count);
		Entry.SetValue(Numbers.Like(StepRule.Apply(current, step, direction, Min, Max), Entry.GetValue()));
	}

	private void SetBar(int bar)
	{
		if (Entry.Available)
		{
			Entry.SetValue(Numbers.Like(RangeBars.ValueForBar(bar, Min, Max), Entry.GetValue()));
		}
	}

	public bool Apply(string text)
	{
		if (!Entry.Available || !NumberParser.TryParse(text, Entry.Number.Integer, Min, Max, out var value))
		{
			return false;
		}
		Entry.SetValue(Numbers.Like(value, Entry.GetValue()));
		return true;
	}

	public override void ReadValue()
	{
		var value = Entry.GetValue();
		_editor.Display.text = InlineEditor.Format(value);
		var filled = RangeBars.Filled(Numbers.ToDouble(value), Min, Max);
		for (var i = 0; i < _bars.Length; i++)
		{
			_bars[i].sprite = GameAssets.Sprite(i < filled ? AssetNames.BarFilled : AssetNames.BarEmpty);
		}
	}
}
