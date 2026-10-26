using System;
using UnityEngine;

namespace PlusModMenu.Core.Unity.Extensions;

public static class RectTransformEx
{
	public enum Anchor
	{
		TopLeft,
		TopCenter,
		TopRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		BottomLeft,
		BottomCenter,
		BottomRight,
		StretchVerticalLeft,
		StretchVerticalCenter,
		StretchVerticalRight,
		StretchHorizontalTop,
		StretchHorizontalMiddle,
		StretchHorizontalBottom,
		StretchAll
	}

	public static void SetAnchor(this RectTransform @this, Anchor anchor)
	{
		switch (anchor)
		{
		case Anchor.TopLeft:
			@this.anchorMin = new Vector2(0f, 1f);
			@this.anchorMax = new Vector2(0f, 1f);
			break;
		case Anchor.TopCenter:
			@this.anchorMin = new Vector2(0.5f, 1f);
			@this.anchorMax = new Vector2(0.5f, 1f);
			break;
		case Anchor.TopRight:
			@this.anchorMin = Vector2.one;
			@this.anchorMax = Vector2.one;
			break;
		case Anchor.MiddleLeft:
			@this.anchorMin = new Vector2(0f, 0.5f);
			@this.anchorMax = new Vector2(0f, 0.5f);
			break;
		case Anchor.MiddleCenter:
			@this.anchorMin = new Vector2(0.5f, 0.5f);
			@this.anchorMax = new Vector2(0.5f, 0.5f);
			break;
		case Anchor.MiddleRight:
			@this.anchorMin = new Vector2(1f, 0.5f);
			@this.anchorMax = new Vector2(1f, 0.5f);
			break;
		case Anchor.BottomLeft:
			@this.anchorMin = Vector2.zero;
			@this.anchorMax = Vector2.zero;
			break;
		case Anchor.BottomCenter:
			@this.anchorMin = new Vector2(0.5f, 0f);
			@this.anchorMax = new Vector2(0.5f, 0f);
			break;
		case Anchor.BottomRight:
			@this.anchorMin = new Vector2(1f, 0f);
			@this.anchorMax = new Vector2(1f, 0f);
			break;
		case Anchor.StretchVerticalLeft:
			@this.anchorMin = Vector2.zero;
			@this.anchorMax = new Vector2(0f, 1f);
			break;
		case Anchor.StretchVerticalCenter:
			@this.anchorMin = new Vector2(0.5f, 0f);
			@this.anchorMax = new Vector2(0.5f, 1f);
			break;
		case Anchor.StretchVerticalRight:
			@this.anchorMin = new Vector2(1f, 0f);
			@this.anchorMax = Vector2.one;
			break;
		case Anchor.StretchHorizontalTop:
			@this.anchorMin = new Vector2(0f, 1f);
			@this.anchorMax = Vector2.one;
			break;
		case Anchor.StretchHorizontalMiddle:
			@this.anchorMin = new Vector2(0f, 0.5f);
			@this.anchorMax = new Vector2(1f, 0.5f);
			break;
		case Anchor.StretchHorizontalBottom:
			@this.anchorMin = Vector2.zero;
			@this.anchorMax = new Vector2(1f, 0f);
			break;
		case Anchor.StretchAll:
			@this.anchorMin = Vector2.zero;
			@this.anchorMax = Vector2.one;
			break;
		default:
			throw new ArgumentOutOfRangeException("anchor", anchor, null);
		}
	}
}
