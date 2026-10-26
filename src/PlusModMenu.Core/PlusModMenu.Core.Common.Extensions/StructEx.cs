namespace PlusModMenu.Core.Common.Extensions;

public static class StructEx
{
	public static bool IsDefault<T>(this T value) where T : struct
	{
		return value.Equals(default(T));
	}
}
