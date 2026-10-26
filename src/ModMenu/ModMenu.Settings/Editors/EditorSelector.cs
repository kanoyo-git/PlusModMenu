using System;
using System.Linq;
using System.Reflection;

namespace ModMenu.Settings.Editors;

public static class EditorSelector
{
	public static EditorInformation Select(ILookup<Type, EditorInformation> table, Type editableType, Assembly assembly, MemberInfo member)
	{
		bool Fits(EditorInformation editor) => editor.RequiredAttributes.All(a => member.IsDefined(a, true));
		var overrideEditor = member.GetCustomAttribute<OverrideEditorControlAttribute>(true);
		if (overrideEditor != null)
		{
			return table[null].Where(x => x.EditorType == overrideEditor.EditorType).FirstOrDefault(Fits);
		}
		if (editableType == null)
		{
			return null;
		}
		var candidates = table[editableType];
		var editor = candidates.Where(Fits).FirstOrDefault(x => x.EditorType.Assembly == assembly) ?? candidates.FirstOrDefault(Fits);
		if (editor == null && editableType.IsEnum)
		{
			editor = table[typeof(Enum)].FirstOrDefault(Fits);
		}
		return editor;
	}
}
