using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ModMenu.Settings.Editors;

public class EditorInformation
{
	private static ILookup<Type, EditorInformation> _editorControlTable = new EditorInformation[0].ToLookup(x => x.EditableType);

	public Type EditorType { get; init; }

	public Type EditableType { get; init; }

	public Type[] RequiredAttributes { get; init; }

	private EditorInformation()
	{
	}

	public static void UpdateEditorControlTable(IEnumerable<Assembly> assemblies)
	{
		_editorControlTable = BuildTable(assemblies.SelectMany(LoadableTypes));
	}

	public static ILookup<Type, EditorInformation> BuildTable(IEnumerable<Type> types)
	{
		return (from type in types
			where typeof(IEditor).IsAssignableFrom(type) && !type.IsAbstract && !type.IsGenericType && type.IsDefined(typeof(EditorControlAttribute), false)
			from attribute in type.GetCustomAttributes<EditorControlAttribute>(false)
			select new EditorInformation
			{
				EditorType = type,
				EditableType = attribute.EditableType,
				RequiredAttributes = attribute.RequiredAttributes
			}).OrderByDescending(x => x.RequiredAttributes.Length).ToLookup(x => x.EditableType);
	}

	public static EditorInformation GetEditorControl(Type editableType, Assembly assembly, MemberInfo member)
	{
		return EditorSelector.Select(_editorControlTable, editableType, assembly, member);
	}

	private static IEnumerable<Type> LoadableTypes(Assembly assembly)
	{
		try
		{
			return assembly.GetTypes();
		}
		catch (ReflectionTypeLoadException e)
		{
			return e.Types.Where(t => t != null);
		}
	}
}
