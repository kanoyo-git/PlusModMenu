using System;

namespace PlusModMenu.Api.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public class GroupAttribute<TGroup> : GroupAttribute
{
	public GroupAttribute()
		: base(typeof(TGroup))
	{
	}
}
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public class GroupAttribute : Attribute
{
	public Type Group { get; }

	public GroupAttribute(Type groupType)
	{
		Group = groupType;
	}
}
