using System;
using System.Collections.Generic;
using ModMenu.BaldiPlusEngineGame.Clipboard.Rows;
using ModMenu.Settings;
using PlusModMenu.Api.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModMenu.BaldiPlusEngineGame.Clipboard;

internal sealed class DialogLayer : MonoBehaviour, IDialogs
{
	private const float Padding = 8f;

	private const float ButtonHeight = 24f;

	private const float ListRow = 22f;

	private const int ListRows = 8;

	private readonly List<GameObject> _windows = new List<GameObject>();

	private readonly List<List<Hotspot>> _buttons = new List<List<Hotspot>>();

	private readonly List<Action> _onCancel = new List<Action>();

	private readonly Queue<Action> _pending = new Queue<Action>();

	private RectTransform _root;

	public bool IsOpen => _windows.Count > 0;

	public IReadOnlyList<Hotspot> TopButtons => _buttons.Count > 0 ? _buttons[_buttons.Count - 1] : (IReadOnlyList<Hotspot>)Array.Empty<Hotspot>();

	public void Attach(RectTransform canvas)
	{
		_root = UiFactory.Rect("Dialogs", canvas);
		_root.Stretch();
	}

	public void ShowPending()
	{
		while (_pending.Count > 0 && _root != null && ClipboardMenu.IsOpen)
		{
			_pending.Dequeue()();
		}
	}

	public void CloseTop()
	{
		if (_windows.Count == 0)
		{
			return;
		}
		var window = _windows[_windows.Count - 1];
		_windows.RemoveAt(_windows.Count - 1);
		_buttons.RemoveAt(_buttons.Count - 1);
		var onCancel = _onCancel[_onCancel.Count - 1];
		_onCancel.RemoveAt(_onCancel.Count - 1);
		window.SetActive(false);
		Destroy(window);
		onCancel?.Invoke();
	}

	public void CloseAll()
	{
		while (IsOpen)
		{
			CloseTop();
		}
	}

	public void Message(UIText title, UIText text)
	{
		Show(() => Window(title, text, 0f, null, new (UIText, Action)[1] { (UIText.Resource(typeof(Strings), "CLOSE"), CloseTop) }));
	}

	public void Confirm(UIText title, UIText text, UIText confirmLabel, Action onConfirm)
	{
		Show(() => Window(title, text, 0f, null, new (UIText, Action)[2]
		{
			(UIText.Resource(typeof(Strings), "CANCEL"), CloseTop),
			(confirmLabel, () =>
			{
				CloseTop();
				onConfirm?.Invoke();
			})
		}));
	}

	public void Prompt(UIText title, UIText text, string initialValue, UIText confirmLabel, Action<string> onConfirm, Action onCancel = null)
	{
		Show(() =>
		{
			TMP_InputField input = null;
			void Submit()
			{
				var value = input.text;
				_onCancel[_onCancel.Count - 1] = null;
				CloseTop();
				onConfirm?.Invoke(value);
			}
			Window(title, text, 28f, (content, width) =>
			{
				input = UiFactory.Input("Input", content, AssetNames.Font18, width, 22f);
				input.GetComponent<RectTransform>().PlaceTopLeft(0f, 2f, width, 22f);
				input.onFocusSelectAll = true;
				input.SetTextWithoutNotify(initialValue ?? string.Empty);
				input.onSubmit.AddListener(_ => Submit());
			}, new (UIText, Action)[2]
			{
				(UIText.Resource(typeof(Strings), "CANCEL"), CloseTop),
				(confirmLabel, Submit)
			});
			_onCancel[_onCancel.Count - 1] = onCancel;
			input.ActivateInputField();
			input.Select();
		});
	}

	public void Choose(UIText title, IReadOnlyList<UIText> options, Action<int> onChoose)
	{
		Show(() =>
		{
			var count = options?.Count ?? 0;
			Window(title, default, Mathf.Min(count, ListRows) * ListRow + 2f, (content, width) =>
			{
				var viewport = UiFactory.Rect("List", content);
				viewport.PlaceTopLeft(0f, 0f, width, Mathf.Min(count, ListRows) * ListRow + 2f);
				UiFactory.HitArea(viewport);
				viewport.gameObject.AddComponent<RectMask2D>();
				var list = UiFactory.Rect("Content", viewport);
				list.anchorMin = new Vector2(0f, 1f);
				list.anchorMax = new Vector2(1f, 1f);
				list.pivot = new Vector2(0.5f, 1f);
				list.anchoredPosition = Vector2.zero;
				list.sizeDelta = new Vector2(0f, count * ListRow);
				for (var i = 0; i < count; i++)
				{
					var index = i;
					var option = UiFactory.Text("Option" + i, list, AssetNames.Font18, SettingRow.Text(options[i]));
					option.rectTransform.PlaceTopLeft(4f, i * ListRow, width - 8f, ListRow);
					UiFactory.Ellipsis(option);
					var hotspot = UiFactory.Clickable(option, () =>
					{
						CloseTop();
						onChoose?.Invoke(index);
					}, hover => option.color = hover ? UiFactory.Red : UiFactory.Ink);
					_buttons[_buttons.Count - 1].Add(hotspot);
				}
				var scroll = viewport.gameObject.AddComponent<ScrollRect>();
				scroll.content = list;
				scroll.viewport = viewport;
				scroll.horizontal = false;
				scroll.movementType = ScrollRect.MovementType.Clamped;
				scroll.inertia = false;
				scroll.scrollSensitivity = ListRow;
			}, new (UIText, Action)[1] { (UIText.Resource(typeof(Strings), "CANCEL"), CloseTop) });
		});
	}

	private void Show(Action build)
	{
		if (_root == null || !ClipboardMenu.IsOpen)
		{
			_pending.Enqueue(build);
			return;
		}
		build();
	}

	private void Window(UIText title, UIText text, float extraHeight, Action<RectTransform, float> extra, (UIText label, Action click)[] buttons)
	{
		var menu = ClipboardMenu.Instance;
		var width = Mathf.Min(300f, (menu != null ? menu.Layout.ClipboardWidth : 480f) - 60f);
		var inner = width - 2f * Padding;
		var blocker = UiFactory.Rect("Dialog", _root);
		blocker.Stretch();
		UiFactory.Clickable(UiFactory.HitArea(blocker), CloseTop).Feedback = false;
		_windows.Add(blocker.gameObject);
		_buttons.Add(new List<Hotspot>());
		_onCancel.Add(null);
		var frame = UiFactory.Frame("Window", blocker);
		frame.raycastTarget = true;
		var window = frame.rectTransform;
		var y = Padding;
		var titleText = UiFactory.Text("Title", window, AssetNames.Font18, SettingRow.Text(title), TextAlignmentOptions.Center);
		UiFactory.Ellipsis(titleText);
		titleText.rectTransform.PlaceTopLeft(Padding, y, inner, 22f);
		y += 24f;
		var body = text.Resolve();
		if (!string.IsNullOrEmpty(body))
		{
			var bodyText = UiFactory.Text("Text", window, AssetNames.Font12, body, TextAlignmentOptions.TopLeft);
			bodyText.enableWordWrapping = true;
			var height = Mathf.Min(180f, Mathf.Ceil(bodyText.GetPreferredValues(body, inner, 0f).y));
			bodyText.overflowMode = TextOverflowModes.Ellipsis;
			bodyText.rectTransform.PlaceTopLeft(Padding, y, inner, height);
			y += height + 6f;
		}
		if (extra != null)
		{
			var content = UiFactory.Rect("Content", window);
			content.PlaceTopLeft(Padding, y, inner, extraHeight);
			extra(content, inner);
			y += extraHeight + 6f;
		}
		var x = width - Padding;
		for (var i = buttons.Length - 1; i >= 0; i--)
		{
			var (label, click) = buttons[i];
			var button = UiFactory.Frame("Button" + i, window);
			var caption = UiFactory.Text("Text", button.rectTransform, AssetNames.Font18, SettingRow.Text(label), TextAlignmentOptions.Center);
			caption.rectTransform.Stretch();
			var buttonWidth = Mathf.Ceil(caption.GetPreferredValues(caption.text).x) + 16f;
			x -= buttonWidth;
			button.rectTransform.PlaceTopLeft(x, y, buttonWidth, ButtonHeight);
			x -= 6f;
			_buttons[_buttons.Count - 1].Insert(0, UiFactory.Clickable(button, click));
		}
		y += ButtonHeight + Padding;
		var offset = menu != null && menu.Board != null ? (Vector2)menu.Board.localPosition : Vector2.zero;
		window.Place(offset + new Vector2(0f, 10f), new Vector2(width, y));
	}
}
