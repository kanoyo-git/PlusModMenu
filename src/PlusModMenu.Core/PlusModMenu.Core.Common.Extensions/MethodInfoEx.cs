using System.Reflection;

namespace PlusModMenu.Core.Common.Extensions;

public static class MethodInfoEx
{
	public static bool IsOverride(this MethodInfo @this)
	{
		return @this.GetBaseDefinition().DeclaringType != @this.DeclaringType;
	}

	public static T Invoke<T>(this MethodBase @this, object obj, params object[] parameters)
	{
		return (T)@this.Invoke(obj, parameters);
	}
}
