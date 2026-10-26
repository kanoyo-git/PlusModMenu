using System;
using System.Collections.Generic;
using ModMenu.BaldiPlusEngineGame.Clipboard.Rows;
using ModMenu.Settings;
using PlusModMenu.Api.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModMenu.BaldiPlusEngineGame.Clipboard;

internal sealed class SearchResultsView
{
	public const int MaxResults = 50;

	private const float Top = 52f;

	private readonly List<SettingRow> _rows = new List<SettingRow>();

	public IReadOnlyList<SettingRow> Rows => _rows;

	public SearchResultsView(RectTransform parent, ClipboardGeometry geometry, List<SettingEntry> results, Action<SettingEntry> open)
	{
		var left = geometry.TwoColumns ? geometry.ListLeft : geometry.PageLeft;
		var width = geometry.PageRight - left;
		var centre = (left + geometry.PageRight) / 2f;
		var summary = results.Count == 0 ? UIText.Resource(typeof(Strings), "SEARCH_NOTHING")
			: results.Count > MaxResults ? UIText.Resource(typeof(Strings), "SEARCH_RESULTS_LIMITED", MaxResults, results.Count)
			: UIText.Resource(typeof(Strings), "SEARCH_RESULTS", results.Count);
		var caption = UiFactory.Text("Summary", parent, AssetNames.Font12, SettingRow.Text(summary), TextAlignmentOptions.Center);
		caption.color = UiFactory.Grey;
		caption.rectTransform.Place(new Vector2(centre, Top - 8f), new Vector2(width, 16f));
		var viewport = UiFactory.Rect("Results", parent);
		var viewTop = Top - 18f;
		viewport.Place(new Vector2(centre, (viewTop + ClipboardGeometry.PaperBottom) / 2f), new Vector2(width, viewTop - ClipboardGeometry.PaperBottom));
		UiFactory.HitArea(viewport);
		viewport.gameObject.AddComponent<RectMask2D>();
		var content = UiFactory.Rect("Content", viewport);
		content.anchorMin = new Vector2(0f, 1f);
		content.anchorMax = new Vector2(1f, 1f);
		content.pivot = new Vector2(0.5f, 1f);
		content.anchoredPosition = Vector2.zero;
		var y = 0f;
		var rowWidth = width - 16f;
		for (var i = 0; i < results.Count && i < MaxResults; i++)
		{
			var entry = results[i];
			var row = SettingRow.Create(entry, content, rowWidth);
			row.Rect.anchoredPosition = new Vector2(0f, 0f - y);
			if (entry.Kind == SettingKind.Action)
			{
				((ActionRow)row).MakeCompact();
				row.Rect.anchoredPosition = new Vector2(SettingRow.LabelX, 0f - y - 2f);
			}
			else
			{
				row.LabelClick = () => open(entry);
				AddPath(row, entry);
			}
			y += SettingRow.Height;
			_rows.Add(row);
		}
		if (results.Count > 0)
		{
			var hint = UiFactory.Text("Hint", content, AssetNames.Font12, SettingRow.Text(UIText.Resource(typeof(Strings), "SEARCH_HINT")), TextAlignmentOptions.Center);
			hint.color = UiFactory.Grey;
			hint.rectTransform.PlaceTopLeft(0f, y + 6f, rowWidth, 16f);
			y += 26f;
		}
		content.sizeDelta = new Vector2(0f, y);
		var scroll = viewport.gameObject.AddComponent<ScrollRect>();
		scroll.content = content;
		scroll.viewport = viewport;
		scroll.horizontal = false;
		scroll.movementType = ScrollRect.MovementType.Clamped;
		scroll.inertia = false;
		scroll.scrollSensitivity = SettingRow.Height;
	}

	private static void AddPath(SettingRow row, SettingEntry entry)
	{
		var label = row.NameText;
		var room = label.rectTransform.sizeDelta.x;
		var nameWidth = Mathf.Min(Mathf.Ceil(label.GetPreferredValues(label.text).x) + 2f, room * 0.6f);
		label.rectTransform.sizeDelta = new Vector2(nameWidth, label.rectTransform.sizeDelta.y);
		var page = entry.Group.Page;
		var path = UiFactory.Text("Path", row.Rect, AssetNames.Font12, SearchPath.Format(SettingRow.Text(page.Section.Label), SettingRow.Text(page.Label), SettingRow.Text(entry.Group.Label)));
		path.color = UiFactory.Grey;
		UiFactory.Ellipsis(path);
		path.rectTransform.PlaceTopLeft(SettingRow.LabelX + nameWidth + 6f, 1f, Mathf.Max(10f, room - nameWidth - 6f), SettingRow.Height);
	}
}
