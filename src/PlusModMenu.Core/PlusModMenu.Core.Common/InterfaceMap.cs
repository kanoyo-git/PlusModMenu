using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using PlusModMenu.Core.Common.Extensions;

namespace PlusModMenu.Core.Common;

public class InterfaceMap
{
	private const BindingFlags ALL_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

	public MethodInfo[] InterfaceMethods { get; private init; }

	public PropertyInfo[] InterfaceProperties { get; private set; }

	public MemberInfo[] InterfaceMembers { get; private set; }

	public Type InterfaceType { get; private init; }

	public MethodInfo[] TargetMethods { get; private init; }

	public PropertyInfo[] TargetProperties { get; private set; }

	public MemberInfo[] TargetMembers { get; private set; }

	public Type TargetType { get; private init; }

	private InterfaceMap()
	{
	}

	public static InterfaceMap Get(Type target, Type @interface)
	{
		InterfaceMapping interfaceMap = target.GetInterfaceMap(@interface);
		PropertyInfo[] interfaceProperties = @interface.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		PropertyInfo[] targetProperties = target.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		InterfaceMap interfaceMap2 = new InterfaceMap
		{
			InterfaceMethods = interfaceMap.InterfaceMethods,
			InterfaceType = interfaceMap.InterfaceType,
			TargetMethods = interfaceMap.TargetMethods,
			TargetType = interfaceMap.TargetType
		};
		var source = from x in interfaceMap.InterfaceMethods
			group x by Regex.Replace(x.Name, "^get_|^set_", "") into x
			where x.Count() == 2 && x.All((MethodInfo y) => y.Name.StartsWith("get_") || y.Name.StartsWith("set_"))
			select new
			{
				Get = x.First((MethodInfo y) => y.Name.StartsWith("get_")),
				Set = x.First((MethodInfo y) => y.Name.StartsWith("set_"))
			};
		var source2 = from x in interfaceMap.TargetMethods
			group x by Regex.Replace(x.Name, "^get_|^set_", "") into x
			where x.Count() == 2 && x.All((MethodInfo y) => y.Name.StartsWith("get_") || y.Name.StartsWith("set_"))
			select new
			{
				Get = x.First((MethodInfo y) => y.Name.StartsWith("get_")),
				Set = x.First((MethodInfo y) => y.Name.StartsWith("set_"))
			};
		interfaceMap2.InterfaceProperties = (from x in source
			select interfaceProperties.FirstOrDefault((PropertyInfo y) => y.GetMethod == x.Get && y.SetMethod == x.Set) into x
			where x != null
			select x).ToArray();
		interfaceMap2.TargetProperties = (from x in source2
			select targetProperties.FirstOrDefault((PropertyInfo y) => y.GetMethod == x.Get && y.SetMethod == x.Set) into x
			where x != null
			select x).ToArray();
		interfaceMap2.InterfaceMembers = EnumerableEx.Concat(new IEnumerable<MemberInfo>[2] { interfaceMap2.InterfaceProperties, interfaceMap2.InterfaceMethods }).ToArray();
		interfaceMap2.TargetMembers = EnumerableEx.Concat(new IEnumerable<MemberInfo>[2] { interfaceMap2.TargetProperties, interfaceMap2.TargetMethods }).ToArray();
		return interfaceMap2;
	}
}
