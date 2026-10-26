using System;
using ModMenu.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModMenu.BaldiPlusEngineGame.Clipboard;

internal sealed class HeaderBar
{
	private const float TitleWidth = 124f;

	private const float NeighbourWidth = 76f;

	private const float NeighbourCentre = 100f + NeighbourWidth / 2f;

	private readonly TextMeshProUGUI _title;

	private readonly TextMeshProUGUI _previous;

	private readonly TextMeshProUGUI _next;

	public HeaderBar(RectTransform board, RectTransform canvas, Action previous, Action next, Action close)
	{
		_title = UiFactory.Text("SectionTitle", board, AssetNames.Font24, string.Empty, TextAlignmentOptions.Center);
		_title.rectTransform.Place(new Vector2(0f, 102f), new Vector2(TitleWidth, 50f));
		_title.enableAutoSizing = true;
		_title.fontSizeMin = 18f;
		_title.fontSizeMax = 24f;
		UiFactory.Ellipsis(_title);
		_previous = UiFactory.Text("PreviousSection", board, AssetNames.Font12, string.Empty, TextAlignmentOptions.Right);
		_previous.rectTransform.Place(new Vector2(-NeighbourCentre, 102f), new Vector2(NeighbourWidth, 50f));
		UiFactory.Ellipsis(_previous);
		_next = UiFactory.Text("NextSection", board, AssetNames.Font12, string.Empty, TextAlignmentOptions.Left);
		_next.rectTransform.Place(new Vector2(NeighbourCentre, 102f), new Vector2(NeighbourWidth, 50f));
		UiFactory.Ellipsis(_next);
		Arrow("PreviousArrow", board, new Vector2(-80f, 110f), left: true, previous);
		Arrow("NextArrow", board, new Vector2(80f, 110f), left: false, next);
		var back = UiFactory.Image("Back", canvas, GameAssets.Sprite(AssetNames.Back));
		back.rectTransform.anchorMin = back.rectTransform.anchorMax = back.rectTransform.pivot = new Vector2(0f, 1f);
		back.rectTransform.anchoredPosition = new Vector2(4f, -4f);
		back.rectTransform.sizeDelta = new Vector2(32f, 32f);
		UiFactory.Clickable(back, close, hover => back.sprite = GameAssets.Sprite(hover ? AssetNames.BackHover : AssetNames.Back));
	}

	public void Show(string previous, string current, string next)
	{
		_previous.text = previous;
		_title.text = current;
		_next.text = next;
	}

	public static Image Arrow(string name, Transform parent, Vector2 position, bool left, Action click)
	{
		var dim = GameAssets.Sprite(left ? AssetNames.ArrowLeftDim : AssetNames.ArrowRightDim);
		var bright = GameAssets.Sprite(left ? AssetNames.ArrowLeft : AssetNames.ArrowRight);
		var image = UiFactory.Image(name, parent, dim);
		image.rectTransform.Place(position, new Vector2(32f, 32f));
		UiFactory.Clickable(image, click, hover => image.sprite = hover ? bright : dim);
		return image;
	}
}
