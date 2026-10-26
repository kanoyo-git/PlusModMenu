using System;
using System.Linq;
using System.Reflection;

namespace PlusModMenu.Core.Common.Extensions;

public static class MemberInfoEx
{
	public static TAttribute? GetAttribute<TAttribute>(this MemberInfo memberInfo) where TAttribute : Attribute
	{
		return Attribute.GetCustomAttribute(memberInfo, typeof(TAttribute), inherit: true) as TAttribute;
	}

	public static bool HasAttributes(this MemberInfo memberInfo, params Type[] attributes)
	{
		return attributes.All(memberInfo.IsDefined);
	}

	public static bool IsVirtual(this MemberInfo @this)
	{
		if (@this == null)
		{
			throw new ArgumentNullException("this");
		}
		if (!(@this is MethodBase { IsVirtual: var isVirtual }))
		{
			if (@this is PropertyInfo propertyInfo)
			{
				return propertyInfo.GetAccessors(nonPublic: true).All((MethodInfo x) => x.IsVirtual);
			}
			return false;
		}
		return isVirtual;
	}

	public static bool IsAbstract(this MemberInfo @this)
	{
		if (@this == null)
		{
			throw new ArgumentNullException("this");
		}
		if (!(@this is MethodBase { IsAbstract: var isAbstract }))
		{
			if (@this is PropertyInfo propertyInfo)
			{
				return propertyInfo.GetAccessors(nonPublic: true).All((MethodInfo x) => x.IsAbstract);
			}
			return false;
		}
		return isAbstract;
	}

	public static Type? ReturnType(this MemberInfo @this)
	{
		if (@this == null)
		{
			throw new ArgumentNullException("this");
		}
		if (!(@this is MethodInfo { ReturnType: var returnType }))
		{
			if (!(@this is PropertyInfo { PropertyType: var propertyType }))
			{
				if (!(@this is FieldInfo { FieldType: var fieldType }))
				{
					return null;
				}
				return fieldType;
			}
			return propertyType;
		}
		return returnType;
	}
}
