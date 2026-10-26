using Mono.Cecil;

namespace PlusModMenu.Codegen;

public class GeneratedProperty
{
	public string Name { get; private init; }

	public GeneratedType Type { get; private init; }

	public MethodDefinition GetMethodDefinition { get; private init; }

	public MethodDefinition SetMethodDefinition { get; private init; }

	private GeneratedProperty()
	{
	}

	public static GeneratedProperty CreateNew(string name, MethodDefinition getMethodDefinition, MethodDefinition setMethodDefinition, GeneratedType type)
	{
		return new GeneratedProperty
		{
			Name = name,
			Type = type,
			GetMethodDefinition = getMethodDefinition,
			SetMethodDefinition = setMethodDefinition
		};
	}
}
