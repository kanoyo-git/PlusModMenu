using ModMenu.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModMenu.BaldiPlusEngineGame.Clipboard.Rows;

internal sealed class ChoiceRow : SettingRow
{
	public const int DropdownFrom = 9;

	private TextMeshProUGUI _value;

	private Image _previous;

	private Image _next;

	private Image _open;

	private bool? _dropdown;

	protected override void Build()
	{
		_previous = HeaderBar.Arrow("Previous", Control, new Vector2(-60f, 0f), left: true, () => Step(-1));
		_next = HeaderBar.Arrow("Next", Control, new Vector2(60f, 0f), left: false, () => Step(1));
		_open = HeaderBar.Arrow("Open", Control, new Vector2(60f, 0f), left: false, Open);
		_open.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -90f);
		_value = UiFactory.Text("Value", Control, AssetNames.Font18, string.Empty, TextAlignmentOptions.Center);
		UiFactory.Ellipsis(_value);
		UiFactory.Clickable(_value, () =>
		{
			if (_dropdown == true)
			{
				Open();
			}
		}, hover => _value.color = hover && _dropdown == true ? UiFactory.Red : UiFactory.Ink);
		Layout(false);
	}

	public bool IsDropdown => _dropdown == true;

	private void Layout(bool dropdown)
	{
		if (_dropdown == dropdown)
		{
			return;
		}
		_dropdown = dropdown;
		_previous.gameObject.SetActive(!dropdown);
		_next.gameObject.SetActive(!dropdown);
		_open.gameObject.SetActive(dropdown);
		Place(wide: false);
	}

	private void Place(bool wide) =>
		_value.rectTransform.Place(wide ? new Vector2(-16f, 0f) : Vector2.zero, new Vector2(wide ? 120f : 88f, Height));

	private void Step(int direction)
	{
		var count = Entry.Options?.Invoke()?.Count ?? 0;
		if (!Entry.Available || count == 0)
		{
			return;
		}
		var index = Entry.GetValue() is int i ? i : 0;
		Entry.SetValue(((index + direction) % count + count) % count);
	}

	public void Open()
	{
		var options = Entry.Options?.Invoke();
		if (!Entry.Available || options == null || options.Count == 0 || ClipboardMenu.Instance == null)
		{
			return;
		}
		ClipboardMenu.Instance.Dialogs.Choose(Entry.Label, options, index => Entry.SetValue(index));
	}

	public override void ReadValue()
	{
		var options = Entry.Options?.Invoke();
		Layout(options != null && options.Count >= DropdownFrom);
		var index = Entry.GetValue() is int i ? i : -1;
		_value.text = options != null && index >= 0 && index < options.Count ? Text(options[index]) : string.Empty;
		Place(wide: _dropdown == true && _value.GetPreferredValues(_value.text).x > 88f);
	}
}
