using ModMenu.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace ModMenu.BaldiPlusEngineGame.Clipboard;

internal sealed class HandCursor : MonoBehaviour
{
	private static readonly Vector2 Fingertip = new Vector2(23.5f / 64f, 1f - 16f / 64f);

	private RectTransform _canvas;

	private RectTransform _rect;

	public static HandCursor Create(RectTransform canvas)
	{
		var sprite = GameAssets.Sprite(AssetNames.Cursor);
		var image = UiFactory.Image("HandCursor", canvas, sprite);
		var cursor = image.gameObject.AddComponent<HandCursor>();
		cursor._canvas = canvas;
		cursor._rect = image.rectTransform;
		cursor._rect.anchorMin = cursor._rect.anchorMax = new Vector2(0.5f, 0.5f);
		cursor._rect.sizeDelta = new Vector2(32f, 32f);
		cursor._rect.pivot = Fingertip;
		return cursor;
	}

	private void LateUpdate()
	{
		transform.SetAsLastSibling();
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas, ClipboardMenu.Pointer, null, out var local))
		{
			_rect.anchoredPosition = local;
		}
	}
}
