using System;
using System.Collections;
using ModMenu.Settings;
using TMPro;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Clipboard.Rows;

internal sealed class InlineEditor
{
	private static readonly Color Rejected = new Color(1f, 0.82f, 0.82f, 1f);

	private readonly MonoBehaviour _owner;

	private readonly Func<string> _current;

	private readonly Func<string, bool> _apply;

	private readonly TMP_InputField _input;

	private int _rejectedFrame = -1;

	public TextMeshProUGUI Display { get; }

	public bool Editing => _input.gameObject.activeSelf;

	public InlineEditor(MonoBehaviour owner, RectTransform parent, Vector2 position, float width, float editWidth, string font, Func<string> current, Func<string, bool> apply, Vector2? editPosition = null)
	{
		_owner = owner;
		_current = current;
		_apply = apply;
		Display = UiFactory.Text("Value", parent, font, string.Empty, TextAlignmentOptions.Center);
		Display.rectTransform.Place(position, new Vector2(width, SettingRow.Height));
		UiFactory.Ellipsis(Display);
		UiFactory.Clickable(Display, Begin, hover => Display.color = hover ? UiFactory.Red : UiFactory.Ink);
		_input = UiFactory.Input("Input", parent, font, editWidth, 22f);
		_input.GetComponent<RectTransform>().Place(editPosition ?? position, new Vector2(editWidth, 22f));
		_input.onSubmit.AddListener(text => Submit(text));
		_input.onEndEdit.AddListener(_ => Ended());
		_input.gameObject.SetActive(false);
	}

	public void Begin()
	{
		_input.textComponent.color = UiFactory.Ink;
		_input.text = _current();
		_input.gameObject.SetActive(true);
		Display.enabled = false;
		_input.ActivateInputField();
		_input.Select();
	}

	public bool Submit(string text)
	{
		if (_apply(text))
		{
			End();
			return true;
		}
		_rejectedFrame = Time.frameCount;
		_input.textComponent.color = UiFactory.Red;
		((UnityEngine.UI.Image)_input.targetGraphic).color = Rejected;
		return false;
	}

	public void End()
	{
		((UnityEngine.UI.Image)_input.targetGraphic).color = Color.white;
		_input.gameObject.SetActive(false);
		Display.enabled = true;
	}

	private void Ended()
	{
		var reopen = _rejectedFrame == Time.frameCount && !_input.wasCanceled;
		if (_owner.isActiveAndEnabled)
		{
			_owner.StartCoroutine(EndNextFrame(reopen));
		}
		else
		{
			End();
		}
	}

	private IEnumerator EndNextFrame(bool reopen)
	{
		yield return null;
		if (!Editing)
		{
			yield break;
		}
		if (reopen)
		{
			_input.ActivateInputField();
			_input.Select();
		}
		else
		{
			End();
		}
	}

	public static string Format(object value)
	{
		return ValueFormatter.Format(value);
	}

	public static string FormatExact(object value)
	{
		return ValueFormatter.FormatExact(value);
	}
}
