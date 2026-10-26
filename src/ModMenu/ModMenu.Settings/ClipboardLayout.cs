namespace ModMenu.Settings;

public readonly record struct ClipboardLayout(float CanvasWidth, float ClipboardWidth, bool TwoColumns, float SearchWidth)
{
	public const float CanvasHeight = 360;

	public const float MaxWidth = 640;

	public const float TwoColumnThreshold = 560;

	public const float CompactSearchWidth = 220;

	public const float WideSearchWidth = 300;

	public static ClipboardLayout For(int screenWidth, int screenHeight)
	{
		var canvas = screenWidth > 0 && screenHeight > 0 ? CanvasHeight * screenWidth / screenHeight : CanvasHeight * 4 / 3;
		var clipboard = canvas < MaxWidth ? canvas : MaxWidth;
		var two = clipboard >= TwoColumnThreshold;
		return new ClipboardLayout(canvas, clipboard, two, two ? WideSearchWidth : CompactSearchWidth);
	}
}
