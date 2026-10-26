namespace ModMenu.Hl1;

public static class CarryRules
{
	public const float MaxWidth = 5f;

	public const float MaxHeight = 4.5f;

	public static readonly string[] GameScriptsAllowed = { "RendererContainer", "EnvironmentObject" };

	public static bool CanCarry(float width, float depth, float height, bool hasGameScripts, bool standingOnIt)
	{
		var widest = width > depth ? width : depth;
		return widest <= MaxWidth && height <= MaxHeight && !hasGameScripts && !standingOnIt;
	}

	public static float HoldDistance(float objectExtentAlongView, float freeFraction)
	{
		var radius = 16 * 1.41421356f * Hl1Constants.K + objectExtentAlongView;
		var distance = (24 * Hl1Constants.K + 2 * radius) * freeFraction;
		var min = radius * 0.5f;
		return distance < min ? min : distance;
	}
}
