using ModMenu.Settings;
using TMPro;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Clipboard.Rows;

internal sealed class TextRow : SettingRow
{
	private TMP_InputField _input;

	protected override void Build()
	{
		_input = UiFactory.Input("Input", Control, AssetNames.Font18, 150f, 22f);
		_input.GetComponent<RectTransform>().Place(Vector2.zero, new Vector2(150f, 22f));
		_input.onEndEdit.AddListener(text =>
		{
			if (!_input.wasCanceled && Entry.Available && text != (string)Entry.GetValue())
			{
				Entry.SetValue(text);
			}
		});
	}

	public override void ReadValue()
	{
		if (!_input.isFocused)
		{
			_input.SetTextWithoutNotify((Entry.GetValue() as string) ?? string.Empty);
		}
	}
}
