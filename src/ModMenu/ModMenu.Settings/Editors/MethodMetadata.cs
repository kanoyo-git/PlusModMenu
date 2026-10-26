using System;
using System.Reflection;

namespace ModMenu.Settings.Editors;

public class MethodMetadata : IMetadata
{
	public object Instance { get; init; }

	public MemberInfo Member { get; init; }

	public Action Method { get; init; }

	public MethodMetadata(object instance, MemberInfo member, Action method)
	{
		Instance = instance;
		Member = member;
		Method = method;
	}
}
