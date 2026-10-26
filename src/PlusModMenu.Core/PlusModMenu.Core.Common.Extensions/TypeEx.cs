using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PlusModMenu.Core.Common.Extensions;

public static class TypeEx
{
	private class SimpleTypeComparer : IEqualityComparer<Type>
	{
		public bool Equals(Type? x, Type? y)
		{
			if (x?.Assembly == y?.Assembly && x?.Namespace == y?.Namespace)
			{
				return x?.Name == y?.Name;
			}
			return false;
		}

		public int GetHashCode(Type? obj)
		{
			throw new NotImplementedException();
		}
	}

	public static bool IsGenericFrom(this Type @this, Type? implement)
	{
		if (@this == null)
		{
			throw new ArgumentNullException("this");
		}
		while (implement != null)
		{
			if (implement.IsGenericType && implement.GetGenericTypeDefinition() == @this)
			{
				return true;
			}
			implement = implement.BaseType;
		}
		return false;
	}

	public static MethodInfo? GetGenericMethod(this Type @this, string name, Type[] parameterTypes, Type[]? genericTypes = null)
	{
		foreach (MethodInfo item in from m in @this.GetMethods()
			where m.IsGenericMethod
			where string.Equals(m.Name, name, StringComparison.Ordinal)
			select m)
		{
			Type[] array = parameterTypes;
			if (genericTypes != null)
			{
				if (item.GetGenericArguments().Length != genericTypes.Length)
				{
					continue;
				}
				Dictionary<Type, Type> genericTypeAssignments = genericTypes.Zip(item.GetGenericArguments(), (Type k, Type v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
				array = array.Select((Type x) => (!genericTypeAssignments.TryGetValue(x, out Type value)) ? x : value).ToArray();
			}
			if ((from p in item.GetParameters()
				select p.ParameterType).ToArray().SequenceEqual(array, new SimpleTypeComparer()))
			{
				return item;
			}
		}
		return null;
	}

	public static bool IsAssignableTo<T>(this Type? @this)
	{
		return @this.IsAssignableTo(typeof(T));
	}

	public static bool IsAssignableTo(this Type? @this, Type? targetType)
	{
		return targetType?.IsAssignableFrom(@this) ?? false;
	}
}
