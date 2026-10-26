using ModMenu.Settings;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Clipboard;

internal readonly struct ClipboardGeometry
{
	public const float PaperLeftInset = 62f;

	public const float PaperRightInset = 54f;

	public const float PaperBottom = -172f;

	public readonly bool TwoColumns;

	public readonly float ListLeft;

	public readonly float ListWidth;

	public readonly float PageLeft;

	public readonly float PageRight;

	public readonly float PageTop;

	public float PageWidth => PageRight - PageLeft;

	public ClipboardGeometry(ClipboardLayout layout)
	{
		var width = Mathf.Max(layout.ClipboardWidth, 2f * ClipboardFrame.Edge);
		var paperLeft = (0f - width) / 2f + PaperLeftInset;
		var paperRight = width / 2f - PaperRightInset;
		TwoColumns = layout.TwoColumns;
		if (TwoColumns)
		{
			ListLeft = paperLeft + 8f;
			ListWidth = Mathf.Clamp(width * 0.27f, 130f, 170f);
			PageLeft = ListLeft + ListWidth;
			PageTop = 24f;
		}
		else
		{
			ListLeft = paperLeft + 8f;
			ListWidth = 0f;
			PageLeft = paperLeft + 8f;
			PageTop = 12f;
		}
		PageRight = paperRight - 6f;
	}
}
