using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using PlusModMenu.Core.Common.Extensions;

namespace PlusModMenu.Core.Common;

public class ResourceHelper
{
	private static readonly Dictionary<Type, System.Resources.ResourceManager> _resourceManagers = new Dictionary<Type, System.Resources.ResourceManager>();

	public static string GetString(Type resourceType, string name)
	{
		return ResourceManagerByType(resourceType).GetString(name) ?? name;
	}

	public static string GetString(Type resourceType, string name, CultureInfo cultureInfo)
	{
		return ResourceManagerByType(resourceType).GetString(name, cultureInfo) ?? name;
	}

	public static System.Resources.ResourceManager ResourceManagerByType(Type resourceType)
	{
		if (!_resourceManagers.TryGetValue(resourceType, out System.Resources.ResourceManager value))
		{
			value = resourceType.GetProperty("ResourceManager", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue<System.Resources.ResourceManager>(null) ?? throw new ArgumentException("The provided Type is not a valid resource type.", "resourceManager");
			_resourceManagers.Add(resourceType, value);
		}
		return value;
	}
}
