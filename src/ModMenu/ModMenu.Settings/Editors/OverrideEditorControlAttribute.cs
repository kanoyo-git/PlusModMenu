using System;

namespace ModMenu.Settings.Editors;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class OverrideEditorControlAttribute : Attribute
{
	public Type EditorType { get; }

	public OverrideEditorControlAttribute(Type editorType)
	{
		EditorType = editorType;
	}
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class OverrideEditorControlAttribute<TEditorControl> : OverrideEditorControlAttribute where TEditorControl : IEditor
{
	public OverrideEditorControlAttribute()
		: base(typeof(TEditorControl))
	{
	}
}
