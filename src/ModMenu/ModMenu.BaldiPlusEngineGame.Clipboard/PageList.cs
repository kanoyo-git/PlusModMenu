using System;
using System.Collections.Generic;
using ModMenu.BaldiPlusEngineGame.Clipboard.Rows;
using ModMenu.Settings;
using PlusModMenu.Api.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ModMenu.BaldiPlusEngineGame.Clipboard;

internal sealed class PageList
{
	private const float RowStep = 23f;

	private const float ListTop = 50f;

	private const float CaptionY = -152f;

	private readonly IReadOnlyList<string> _labels;

	private readonly int _selected;

	private readonly Action<int> _pick;

	private readonly RectTransform _root;

	private readonly ClipboardGeometry _geometry;

	private readonly Action<int> _scrolled;

	private int _scroll;

	public int ScrollPosition => _scroll;

	public PageList(RectTransform parent, ClipboardGeometry geometry, IReadOnlyList<string> labels, int selected, Action<int> pick, bool groups, int scroll = 0, Action<int> scrolled = null)
	{
		_scrolled = scrolled;
		_labels = labels;
		_selected = selected;
		_pick = pick;
		_geometry = geometry;
		_root = UiFactory.Rect("PageList", parent);
		_root.Stretch();
		if (geometry.TwoColumns)
		{
			_scroll = PageListPaging.Window(labels.Count, selected, scroll).first;
			_scrolled?.Invoke(_scroll);
			BuildList();
		}
		else
		{
			BuildSelector(groups);
		}
	}

	private void BuildList()
	{
		var area = UiFactory.Rect("List", _root);
		area.Place(new Vector2(_geometry.ListLeft + _geometry.ListWidth / 2f, ListTop - RowStep * PageListPaging.RowsPerScreen / 2f), new Vector2(_geometry.ListWidth, RowStep * PageListPaging.RowsPerScreen));
		UiFactory.HitArea(area);
		area.gameObject.AddComponent<WheelRelay>().Wheel = delta => Scroll(delta > 0f ? -1 : 1);
		var (first, last) = PageListPaging.Window(_labels.Count, -1, _scroll);
		for (var i = first; i <= last; i++)
		{
			var index = i;
			var y = ListTop - RowStep * (i - first) - RowStep / 2f;
			if (i == _selected)
			{
				var frame = UiFactory.Frame("Selected", _root);
				frame.rectTransform.Place(new Vector2(_geometry.ListLeft + _geometry.ListWidth / 2f, y), new Vector2(_geometry.ListWidth, RowStep));
			}
			var label = UiFactory.Text("Page " + _labels[i], _root, AssetNames.Font18, _labels[i]);
			label.rectTransform.Place(new Vector2(_geometry.ListLeft + _geometry.ListWidth / 2f + 2f, y), new Vector2(_geometry.ListWidth - 16f, RowStep));
			UiFactory.Ellipsis(label);
			UiFactory.Clickable(label, () => _pick(index), hover => label.color = hover ? UiFactory.Red : UiFactory.Ink);
		}
		if (_labels.Count > PageListPaging.RowsPerScreen)
		{
			var centre = _geometry.ListLeft + _geometry.ListWidth / 2f;
			var caption = UiFactory.Text("Range", _root, AssetNames.Font12, PageListPaging.Caption(first, last, _labels.Count, UIText.Resource(typeof(Strings), "PAGE_RANGE").Resolve()), TextAlignmentOptions.Center);
			caption.color = UiFactory.Grey;
			caption.rectTransform.Place(new Vector2(centre, CaptionY), new Vector2(_geometry.ListWidth - 60f, 20f));
			HeaderBar.Arrow("ListUp", _root, new Vector2(centre - 44f, CaptionY), left: true, () => Scroll(-PageListPaging.RowsPerScreen)).rectTransform.localEulerAngles = new Vector3(0f, 0f, -90f);
			HeaderBar.Arrow("ListDown", _root, new Vector2(centre + 44f, CaptionY), left: false, () => Scroll(PageListPaging.RowsPerScreen)).rectTransform.localEulerAngles = new Vector3(0f, 0f, -90f);
		}
	}

	private void Scroll(int rows)
	{
		var next = PageListPaging.Window(_labels.Count, -1, _scroll + rows).first;
		if (next == _scroll)
		{
			return;
		}
		_scroll = next;
		_scrolled?.Invoke(_scroll);
		for (var i = _root.childCount - 1; i >= 0; i--)
		{
			UnityEngine.Object.Destroy(_root.GetChild(i).gameObject);
		}
		BuildList();
	}

	private void BuildSelector(bool groups)
	{
		var centre = (_geometry.PageLeft + _geometry.PageRight) / 2f;
		var name = UiFactory.Text("PageName", _root, AssetNames.Font24, _selected >= 0 && _selected < _labels.Count ? _labels[_selected] : string.Empty, TextAlignmentOptions.Center);
		name.rectTransform.Place(new Vector2(centre, 44f), new Vector2(150f, 30f));
		UiFactory.Ellipsis(name);
		if (groups || _labels.Count < 2)
		{
			return;
		}
		HeaderBar.Arrow("PreviousPage", _root, new Vector2(centre - 90f, 42f), left: true, () => _pick((_selected - 1 + _labels.Count) % _labels.Count));
		HeaderBar.Arrow("NextPage", _root, new Vector2(centre + 90f, 42f), left: false, () => _pick((_selected + 1) % _labels.Count));
		var caption = UiFactory.Text("Index", _root, AssetNames.Font12, UIText.Resource(typeof(Strings), "PAGE_INDEX", _selected + 1, _labels.Count).Resolve(), TextAlignmentOptions.Center);
		caption.color = UiFactory.Grey;
		caption.rectTransform.Place(new Vector2(centre, 24f), new Vector2(120f, 16f));
	}
}

internal sealed class WheelRelay : MonoBehaviour, IScrollHandler, IEventSystemHandler
{
	public Action<float> Wheel;

	public void OnScroll(PointerEventData eventData)
	{
		Wheel?.Invoke(eventData.scrollDelta.y);
	}
}
