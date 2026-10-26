using System;
using System.Runtime.CompilerServices;

namespace PlusModMenu.Core.Common.Extensions;

public static class RandomEx
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double Range(this Random @this, double min, double max)
	{
		return @this.NextDouble() * (max - min) + min;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Range(this Random @this, float min, float max)
	{
		return (float)@this.NextDouble() * (max - min) + min;
	}
}
