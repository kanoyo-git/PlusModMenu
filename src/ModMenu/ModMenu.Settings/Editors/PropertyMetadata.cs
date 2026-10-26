using System;
using System.Reflection;

namespace ModMenu.Settings.Editors;

public class PropertyMetadata : IMetadata
{
	public object Instance { get; init; }

	public MemberInfo Member { get; init; }

	public Delegate Get { get; init; }

	public Delegate Set { get; init; }

	public PropertyMetadata(object instance, MemberInfo member, Delegate get, Delegate set)
	{
		Instance = instance;
		Member = member;
		Get = get;
		Set = set;
	}
}

public class PropertyMetadata<TReturnType> : PropertyMetadata
{
	public new Func<TReturnType> Get { get; init; }

	public new Action<TReturnType> Set { get; init; }

	public PropertyMetadata(object instance, MemberInfo member, Func<TReturnType> get, Action<TReturnType> set)
		: base(instance, member, get, set)
	{
		Get = get;
		Set = set;
	}
}
