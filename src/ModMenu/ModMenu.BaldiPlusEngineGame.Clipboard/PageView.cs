using System.Collections.Generic;
using ModMenu.BaldiPlusEngineGame.Clipboard.Rows;
using ModMenu.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModMenu.BaldiPlusEngineGame.Clipboard;

internal sealed class PageView
{
	private const float GroupHeaderHeight = SettingRow.Height;

	private const float ArrowColumn = 28f;

	private const float BlockGap = 8f;

	private static Sprite _dot;

	private readonly List<SettingRow> _rows = new List<SettingRow>();

	private readonly Dictionary<SettingsGroup, float> _groupTops = new Dictionary<SettingsGroup, float>();

	private readonly ScrollRect _scroll;

	private readonly RectTransform _content;

	private readonly RectTransform _viewport;

	public IReadOnlyList<SettingRow> Rows => _rows;

	public Image ResetButton { get; private set; }

	public float ScrollOffset
	{
		get
		{
			return _content.anchoredPosition.y;
		}
		set
		{
			var max = Mathf.Max(0f, _content.sizeDelta.y - _viewport.rect.height);
			var rows = Mathf.Round(value / SettingRow.Height) * SettingRow.Height;
			_content.anchoredPosition = new Vector2(0f, Mathf.Clamp(rows, 0f, max));
		}
	}

	public PageView(RectTransform parent, ClipboardGeometry geometry, SettingsPage page)
	{
		if (geometry.TwoColumns)
		{
			var title = UiFactory.Text("PageTitle", parent, AssetNames.Font24, SettingRow.Text(page.Label), TextAlignmentOptions.Center);
			title.rectTransform.Place(new Vector2((geometry.PageLeft + geometry.PageRight) / 2f, 40f), new Vector2(geometry.PageWidth, 30f));
			UiFactory.Ellipsis(title);
		}
		_viewport = UiFactory.Rect("PageView", parent);
		var height = Mathf.Floor((geometry.PageTop - ClipboardGeometry.PaperBottom) / SettingRow.Height) * SettingRow.Height;
		_viewport.Place(new Vector2((geometry.PageLeft + geometry.PageRight) / 2f, geometry.PageTop - height / 2f), new Vector2(geometry.PageWidth, height));
		UiFactory.HitArea(_viewport);
		_viewport.gameObject.AddComponent<RectMask2D>();
		_content = UiFactory.Rect("Content", _viewport);
		_content.anchorMin = new Vector2(0f, 1f);
		_content.anchorMax = new Vector2(1f, 1f);
		_content.pivot = new Vector2(0.5f, 1f);
		_content.anchoredPosition = Vector2.zero;
		var width = geometry.PageWidth - ArrowColumn;
		var y = Fill(page, width);
		_content.sizeDelta = new Vector2(0f, y);
		_scroll = _viewport.gameObject.AddComponent<ScrollRect>();
		_scroll.content = _content;
		_scroll.viewport = _viewport;
		_scroll.horizontal = false;
		_scroll.vertical = true;
		_scroll.movementType = ScrollRect.MovementType.Clamped;
		_scroll.inertia = false;
		_scroll.scrollSensitivity = SettingRow.Height;
		if (y > _viewport.sizeDelta.y)
		{
			var arrowX = geometry.PageWidth / 2f - ArrowColumn / 2f + 2f;
			Rotate(HeaderBar.Arrow("ScrollUp", _viewport.parent, _viewport.anchoredPosition + new Vector2(arrowX, _viewport.sizeDelta.y / 2f - 10f), left: true, () => ScrollOffset -= 3f * SettingRow.Height));
			Rotate(HeaderBar.Arrow("ScrollDown", _viewport.parent, _viewport.anchoredPosition + new Vector2(arrowX, (0f - _viewport.sizeDelta.y) / 2f + 10f), left: false, () => ScrollOffset += 3f * SettingRow.Height));
		}
	}

	private float Fill(SettingsPage page, float width)
	{
		var y = 0f;
		var actions = new List<SettingEntry>();
		foreach (var group in page.Groups)
		{
			var entries = group.Entries;
			if (entries.Count == 0)
			{
				continue;
			}
			_groupTops[group] = y;
			var label = SettingRow.Text(group.Label);
			var hasRows = false;
			foreach (var entry in entries)
			{
				if (entry.Kind == SettingKind.Action && entry.Owner != null)
				{
					actions.Add(entry);
					continue;
				}
				if (!hasRows && label.Length > 0)
				{
					y = GroupHeader(label, width, y);
					hasRows = true;
				}
				var row = SettingRow.Create(entry, _content, width);
				row.Rect.anchoredPosition = new Vector2(0f, 0f - y);
				y += row.RowHeight;
				_rows.Add(row);
			}
		}
		if (actions.Count > 0)
		{
			y += BlockGap;
			var x = SettingRow.LabelX;
			foreach (var entry in actions)
			{
				var row = (ActionRow)SettingRow.Create(entry, _content, width);
				row.MakeCompact();
				if (x > SettingRow.LabelX && x + row.ButtonWidth > width)
				{
					x = SettingRow.LabelX;
					y += ActionRow.ButtonHeight + 6f;
				}
				row.Rect.anchoredPosition = new Vector2(x, 0f - y);
				x += row.ButtonWidth + BlockGap;
				_rows.Add(row);
			}
			y += ActionRow.ButtonHeight + 6f;
		}
		var resettable = false;
		foreach (var entry in page.Entries)
		{
			resettable |= entry.HasDefault && entry.Setter != null;
		}
		if (resettable)
		{
			y += BlockGap;
			var frame = UiFactory.Frame("ResetPage", _content);
			var text = UiFactory.Text("Text", frame.rectTransform, AssetNames.Font18, SettingRow.Text(PlusModMenu.Api.Settings.UIText.Resource(typeof(Strings), "RESET_PAGE")), TextAlignmentOptions.Center);
			text.rectTransform.Stretch();
			var buttonWidth = Mathf.Ceil(text.GetPreferredValues(text.text).x) + 16f;
			frame.rectTransform.PlaceTopLeft(width - buttonWidth - 8f, y, buttonWidth, ActionRow.ButtonHeight);
			UiFactory.Clickable(frame, page.ResetToDefaults);
			ResetButton = frame;
			y += ActionRow.ButtonHeight + 6f;
		}
		return y + 4f;
	}

	private float GroupHeader(string label, float width, float y)
	{
		var text = UiFactory.Text("Group " + label, _content, AssetNames.Font18, label);
		text.color = UiFactory.Red;
		var textWidth = Mathf.Min(Mathf.Ceil(text.GetPreferredValues(label).x), width - SettingRow.LabelX - 20f);
		text.rectTransform.PlaceTopLeft(SettingRow.LabelX, y, textWidth, GroupHeaderHeight);
		UiFactory.Ellipsis(text);
		var dots = UiFactory.Image("Dots", _content, Dot, Image.Type.Tiled);
		var start = SettingRow.LabelX + textWidth + 6f;
		dots.rectTransform.PlaceTopLeft(start, y + GroupHeaderHeight - 8f, Mathf.Max(0f, width - 8f - start), 1f);
		return y + GroupHeaderHeight;
	}

	public void ScrollTo(SettingsGroup group)
	{
		if (_groupTops.TryGetValue(group, out var top))
		{
			ScrollOffset = top;
		}
	}

	public SettingRow ScrollTo(SettingEntry entry)
	{
		foreach (var row in _rows)
		{
			if (row.Entry == entry)
			{
				var top = 0f - row.Rect.anchoredPosition.y;
				var height = _viewport.rect.height;
				if (top < ScrollOffset || top + row.RowHeight > ScrollOffset + height)
				{
					ScrollOffset = top - height / 3f;
				}
				return row;
			}
		}
		return null;
	}

	private static void Rotate(Image arrow)
	{
		arrow.rectTransform.localEulerAngles = new Vector3(0f, 0f, -90f);
	}

	private static Sprite Dot
	{
		get
		{
			if (_dot == null)
			{
				var texture = new Texture2D(3, 1, TextureFormat.RGBA32, mipChain: false)
				{
					filterMode = FilterMode.Point,
					wrapMode = TextureWrapMode.Repeat
				};
				texture.SetPixels32(new Color32[3]
				{
					UiFactory.Red,
					new Color32(0, 0, 0, 0),
					new Color32(0, 0, 0, 0)
				});
				texture.Apply();
				_dot = Sprite.Create(texture, new Rect(0f, 0f, 3f, 1f), new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.FullRect);
			}
			return _dot;
		}
	}
}
