using ModMenu.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace ModMenu.BaldiPlusEngineGame.Clipboard;

internal sealed class ClipboardFrame
{
	public const float Edge = 140f;

	private const float SourceWidth = 480f;

	public RectTransform Rect { get; }

	public ClipboardFrame(Transform parent)
	{
		Rect = UiFactory.Rect("Clipboard", parent);
		var source = GameAssets.Sprite(AssetNames.Clipboard);
		var height = source.rect.height;
		if (source.rect.width < SourceWidth)
		{
			UiFactory.Image("Whole", Rect, source).rectTransform.Stretch();
			Rect.sizeDelta = new Vector2(SourceWidth, ClipboardLayout.CanvasHeight);
			return;
		}
		var left = UiFactory.Image("Left", Rect, Slice(source, 0f, Edge)).rectTransform;
		left.anchorMin = new Vector2(0f, 0.5f);
		left.anchorMax = new Vector2(0f, 0.5f);
		left.pivot = new Vector2(0f, 0.5f);
		left.anchoredPosition = Vector2.zero;
		var right = UiFactory.Image("Right", Rect, Slice(source, SourceWidth - Edge, Edge)).rectTransform;
		right.anchorMin = new Vector2(1f, 0.5f);
		right.anchorMax = new Vector2(1f, 0.5f);
		right.pivot = new Vector2(1f, 0.5f);
		right.anchoredPosition = Vector2.zero;
		var middle = UiFactory.Image("Middle", Rect, Slice(source, Edge, SourceWidth - 2f * Edge), Image.Type.Tiled).rectTransform;
		middle.anchorMin = new Vector2(0f, 0.5f);
		middle.anchorMax = new Vector2(1f, 0.5f);
		middle.pivot = new Vector2(0.5f, 0.5f);
		middle.offsetMin = new Vector2(Edge, (0f - height) / 2f);
		middle.offsetMax = new Vector2(0f - Edge, height / 2f);
		Rect.Place(Vector2.zero, new Vector2(SourceWidth, height));
	}

	public void SetWidth(float width)
	{
		Rect.sizeDelta = new Vector2(Mathf.Max(width, 2f * Edge), Rect.sizeDelta.y);
	}

	private static Sprite Slice(Sprite source, float x, float width)
	{
		var rect = source.textureRect;
		return Sprite.Create(source.texture, new Rect(rect.x + x, rect.y, width, rect.height), new Vector2(0.5f, 0.5f), source.pixelsPerUnit, 0u, SpriteMeshType.FullRect);
	}
}
