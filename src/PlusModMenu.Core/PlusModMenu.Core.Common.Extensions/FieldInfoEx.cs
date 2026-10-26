using System.Reflection;

namespace PlusModMenu.Core.Common.Extensions;

public static class FieldInfoEx
{
	public static T GetValue<T>(this FieldInfo fieldInfo, object? obj)
	{
		return (T)fieldInfo.GetValue(obj);
	}
}
