using System.Reflection;

namespace ModMenu.Settings.Editors;

public interface IMetadata
{
	object Instance { get; init; }

	MemberInfo Member { get; init; }
}
