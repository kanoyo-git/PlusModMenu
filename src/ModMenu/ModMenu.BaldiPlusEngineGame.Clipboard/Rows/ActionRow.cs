using ModMenu.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModMenu.BaldiPlusEngineGame.Clipboard.Rows;

internal sealed class ActionRow : SettingRow
{
	public const float ButtonHeight = 24f;

	private Image _frame;

	private TextMeshProUGUI _text;

	public float ButtonWidth { get; private set; }

	protected override void Build()
	{
		Label.gameObject.SetActive(false);
		Control.gameObject.SetActive(false);
		_frame = UiFactory.Frame("Button", Rect);
		_text = UiFactory.Text("Text", _frame.rectTransform, AssetNames.Font18, Caption(), TextAlignmentOptions.Center);
		_text.rectTransform.Stretch();
		ButtonWidth = Mathf.Min(Mathf.Ceil(_text.GetPreferredValues(_text.text).x) + 16f, Mathf.Max(40f, Rect.sizeDelta.x - LabelX));
		_frame.rectTransform.PlaceTopLeft(LabelX, (Height - ButtonHeight) / 2f, ButtonWidth, ButtonHeight);
		UiFactory.Ellipsis(_text);
		UiFactory.Clickable(_frame, () =>
		{
			if (LabelClick != null)
			{
				LabelClick();
			}
			else
			{
				Click();
			}
		});
	}

	public void MakeCompact()
	{
		Rect.sizeDelta = new Vector2(ButtonWidth, ButtonHeight);
		_frame.rectTransform.PlaceTopLeft(0f, 0f, ButtonWidth, ButtonHeight);
	}

	protected override void ShowCaption(string text)
	{
		_text.text = text;
	}

	public void Click()
	{
		if (Entry.Available)
		{
			Entry.Invoke?.Invoke();
		}
	}

	public override void ReadValue()
	{
	}
}
