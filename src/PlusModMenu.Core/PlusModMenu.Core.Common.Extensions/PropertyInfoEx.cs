using System.Linq;
using System.Reflection;

namespace PlusModMenu.Core.Common.Extensions;

public static class PropertyInfoEx
{
	public static TValueType GetValue<TValueType>(this PropertyInfo @this, object? obj)
	{
		return (TValueType)@this.GetValue(obj);
	}

	public static bool IsStatic(this PropertyInfo @this, bool nonPublic = false)
	{
		return @this.GetAccessors(nonPublic).Any((MethodInfo x) => x.IsStatic);
	}
}
