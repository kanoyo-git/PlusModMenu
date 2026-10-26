using Mono.Cecil;

namespace PlusModMenu.Core.Mono.Cecil.Extensions;

public static class TypeDefinitionEx
{
	public static TypeDefinition Clone(this TypeDefinition @this)
	{
		return new TypeDefinition(@this.Namespace, @this.Name, @this.Attributes);
	}
}
