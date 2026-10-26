using System;

namespace ModMenu.Settings.Editors;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EditorControlAttribute : Attribute
{
	public Type EditableType { get; }

	public Type[] RequiredAttributes { get; }

	public EditorControlAttribute(Type editableType, params Type[] requiredAttributes)
	{
		EditableType = editableType;
		RequiredAttributes = requiredAttributes ?? new Type[0];
	}
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EditorControlAttribute<TReturnType> : EditorControlAttribute
{
	public EditorControlAttribute(params Type[] requiredAttributes)
		: base(typeof(TReturnType), requiredAttributes)
	{
	}
}
