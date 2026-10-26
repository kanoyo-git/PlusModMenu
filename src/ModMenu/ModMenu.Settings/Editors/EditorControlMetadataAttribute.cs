using System;

namespace ModMenu.Settings.Editors;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class EditorControlMetadataAttribute : Attribute
{
	public string Key { get; }

	public string Value { get; }

	public EditorControlMetadataAttribute(string key, string value)
	{
		Key = key;
		Value = value;
	}
}
