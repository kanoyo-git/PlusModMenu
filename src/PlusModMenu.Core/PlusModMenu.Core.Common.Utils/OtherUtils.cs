using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PlusModMenu.Core.Common.Utils;

public static class OtherUtils
{
	public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
	{
		HashSet<TKey> knownKeys = new HashSet<TKey>();
		foreach (TSource item in source)
		{
			if (knownKeys.Add(keySelector(item)))
			{
				yield return item;
			}
		}
	}

	public static T CloneObject<T>(this T objSource)
	{
		T val = Activator.CreateInstance<T>();
		PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (propertyInfo.CanWrite)
			{
				if (propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType.IsEnum || propertyInfo.PropertyType == typeof(string))
				{
					propertyInfo.SetValue(val, propertyInfo.GetValue(objSource, null), null);
				}
				else
				{
					propertyInfo.SetValue(val, propertyInfo.GetValue(objSource, null)?.CloneObject(), null);
				}
			}
		}
		return val;
	}

	public static Delegate Delegate(this MethodInfo methodInfo, object? target)
	{
		Type delegateType = Expression.GetDelegateType((from x in methodInfo.GetParameters()
			select x.ParameterType).Append(methodInfo.ReturnType).ToArray());
		if (!methodInfo.IsStatic)
		{
			return methodInfo.CreateDelegate(delegateType, target);
		}
		return methodInfo.CreateDelegate(delegateType);
	}
}
