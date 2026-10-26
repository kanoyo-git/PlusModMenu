using System;
using ModMenu.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModMenu.BaldiPlusEngineGame.Clipboard;

internal sealed class Tooltip : MonoBehaviour
{
	public const float Delay = 0.4f;

	private const float MaxWidth = 220f;

	private RectTransform _canvas;

	private RectTransform _frame;

	private TextMeshProUGUI _text;

	private object _owner;

	private Func<string> _content;

	private float _since;

	public bool Shown => _frame.gameObject.activeSelf;

	public string ShownText => Shown ? _text.text : null;

	public static Tooltip Create(RectTransform canvas)
	{
		var layer = UiFactory.Rect("Tooltip", canvas);
		layer.Stretch();
		var tooltip = layer.gameObject.AddComponent<Tooltip>();
		tooltip._canvas = canvas;
		var frame = UiFactory.Frame("Frame", layer);
		tooltip._frame = frame.rectTransform;
		tooltip._frame.anchorMin = tooltip._frame.anchorMax = new Vector2(0.5f, 0.5f);
		tooltip._frame.pivot = new Vector2(0f, 1f);
		tooltip._text = UiFactory.Text("Text", tooltip._frame, AssetNames.Font12, string.Empty, TextAlignmentOptions.TopLeft);
		tooltip._text.rectTransform.Stretch(6f, 4f, 6f, 4f);
		tooltip._text.enableWordWrapping = true;
		tooltip._frame.gameObject.SetActive(false);
		return tooltip;
	}

	public void Request(object owner, Func<string> content)
	{
		_owner = owner;
		_content = content;
		_since = Time.unscaledTime;
		_frame.gameObject.SetActive(false);
	}

	public void Cancel(object owner)
	{
		if (_owner == owner)
		{
			_owner = null;
			_content = null;
			_frame.gameObject.SetActive(false);
		}
	}

	public void ShowNow()
	{
		_since = float.NegativeInfinity;
		LateUpdate();
	}

	private void LateUpdate()
	{
		if (_content == null)
		{
			return;
		}
		if (!_frame.gameObject.activeSelf)
		{
			if (Time.unscaledTime - _since < Delay)
			{
				return;
			}
			var text = _content();
			if (string.IsNullOrEmpty(text))
			{
				_content = null;
				return;
			}
			_text.text = text;
			var width = Mathf.Min(MaxWidth, Mathf.Ceil(_text.GetPreferredValues(text).x) + 1f);
			var height = Mathf.Ceil(_text.GetPreferredValues(text, width, 0f).y);
			_frame.sizeDelta = new Vector2(width + 12f, height + 8f);
			_frame.gameObject.SetActive(true);
		}
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas, ClipboardMenu.Pointer, null, out var mouse))
		{
			var size = _frame.sizeDelta;
			var half = _canvas.rect.size / 2f;
			var x = Mathf.Min(mouse.x + 12f, half.x - size.x - 2f);
			var y = mouse.y - 18f;
			if (y - size.y < 2f - half.y)
			{
				y = mouse.y + size.y + 4f;
			}
			_frame.anchoredPosition = new Vector2(Mathf.Max(x, 2f - half.x), Mathf.Min(y, half.y - 2f));
		}
	}
}
