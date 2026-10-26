using System;
using PlusModMenu.Core.Common;
using PlusModMenu.Api.Settings;

namespace PlusModMenu.Api.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
public class LabelAttribute<TResourceType> : LabelAttribute
{
	public LabelAttribute(string resourceName)
		: base(typeof(TResourceType), resourceName)
	{
	}
}
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
public class LabelAttribute : Attribute
{
	public string Text { get; }

	public Type ResourceType { get; }

	public string ResourceName { get; }

	public LabelAttribute(string text)
	{
		Text = text;
	}

	public LabelAttribute(Type resourceType, string resourceName)
	{
		Text = ResourceHelper.GetString(resourceType, resourceName);
		ResourceType = resourceType;
		ResourceName = resourceName;
	}

	public UIText ToUIText()
	{
		return ResourceType != null ? UIText.Resource(ResourceType, ResourceName) : UIText.Literal(Text);
	}

	public static implicit operator LabelAttribute(string value)
	{
		return new LabelAttribute(value);
	}
}
