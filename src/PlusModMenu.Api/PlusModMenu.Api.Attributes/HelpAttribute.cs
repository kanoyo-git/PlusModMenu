using System;
using PlusModMenu.Core.Common;
using PlusModMenu.Api.Settings;

namespace PlusModMenu.Api.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public class HelpAttribute<TResourceType> : HelpAttribute
{
	public HelpAttribute(string titleResourceName, string textResourceName)
		: base(typeof(TResourceType), titleResourceName, textResourceName)
	{
	}
}
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public class HelpAttribute : Attribute
{
	public string Title { get; }

	public string Text { get; }

	public Type ResourceType { get; }

	public string TextResourceName { get; }

	public UIText TextUI => ResourceType != null ? UIText.Resource(ResourceType, TextResourceName) : UIText.Literal(Text);

	public HelpAttribute(string title, string text)
	{
		Title = title;
		Text = text;
	}

	public HelpAttribute(Type resourceType, string titleResourceName, string textResourceName)
	{
		Title = ResourceHelper.GetString(resourceType, titleResourceName);
		Text = ResourceHelper.GetString(resourceType, textResourceName);
		ResourceType = resourceType;
		TextResourceName = textResourceName;
	}
}
