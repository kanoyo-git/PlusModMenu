using System;
using System.Collections;
using System.Reflection;

namespace PlusModMenu.Core.Common.Extensions;

public static class EnumeratorEx
{
	public static TOverlying? GetOverlyingObject<TOverlying>(this IEnumerator @this) where TOverlying : class
	{
		if (@this == null)
		{
			throw new ArgumentNullException("this");
		}
		return @this.GetType().GetField("<>4__this", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(@this) as TOverlying;
	}
}
