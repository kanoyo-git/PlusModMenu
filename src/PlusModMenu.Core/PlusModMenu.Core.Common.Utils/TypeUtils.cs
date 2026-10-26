using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PlusModMenu.Core.Common.Utils;

public class TypeUtils
{
	public static IEnumerable<Type> GetLoadableTypes(IEnumerable<Assembly> assemblies)
	{
		List<Type> list = new List<Type>();
		foreach (Assembly assembly in assemblies)
		{
			try
			{
				list.AddRange(assembly.GetTypes());
			}
			catch (ReflectionTypeLoadException ex)
			{
				list.AddRange(ex.Types.Where((Type t) => t != null));
			}
		}
		return list;
	}

	public static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
	{
		List<Type> list = new List<Type>();
		try
		{
			list.AddRange(assembly.GetTypes());
		}
		catch (ReflectionTypeLoadException ex)
		{
			list.AddRange(ex.Types.Where((Type t) => t != null));
		}
		return list;
	}

	public static IEnumerable<Type> GetTypesImplementing<T>(IEnumerable<Type> types)
	{
		return GetTypesImplementing(types, typeof(T));
	}

	public static IEnumerable<Type> GetTypesImplementing(IEnumerable<Type> types, Type implementingType)
	{
		return types.Where(implementingType.IsAssignableFrom);
	}

	public static IEnumerable<Type> GetFullTypesImplementing(IEnumerable<Type> types, Type implementingType)
	{
		return types.Where((Type x) => implementingType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
	}

	public static bool IsAssignableToGenericType(Type givenType, Type genericType)
	{
		if (givenType.GetInterfaces().Any((Type x) => x.IsGenericType && x.GetGenericTypeDefinition() == genericType))
		{
			return true;
		}
		if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
		{
			return true;
		}
		Type baseType = givenType.BaseType;
		if (baseType != null)
		{
			return IsAssignableToGenericType(baseType, genericType);
		}
		return false;
	}

	public static Type? GetFullTypeFromGeneric(Type? originalType, Type genericType)
	{
		while (originalType?.BaseType != null)
		{
			originalType = originalType.BaseType;
			if (originalType.IsGenericType && originalType.GetGenericTypeDefinition() == genericType)
			{
				return originalType;
			}
		}
		return null;
	}
}
