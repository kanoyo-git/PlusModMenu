using System;
using System.Linq;
using Mono.Cecil;

namespace PlusModMenu.Codegen;

public class GeneratedMethod
{
	public string Name { get; private init; }

	public GeneratedType Type { get; private init; }

	public MethodDefinition MethodDefinition { get; private init; }

	private GeneratedMethod()
	{
	}

	public static GeneratedMethod CreateNew(string name, MethodAttributes methodAttributes, TypeReference returnType, GeneratedType type)
	{
		MethodDefinition methodDefinition = new MethodDefinition(name, methodAttributes, returnType);
		type.TypeDefinition.Methods.Add(methodDefinition);
		return new GeneratedMethod
		{
			Name = name,
			Type = type,
			MethodDefinition = methodDefinition
		};
	}

	public static GeneratedMethod CreateOverride(MethodReference methodReference, GeneratedType type)
	{
		MethodDefinition methodDefinition = methodReference.Resolve();
		if (!methodDefinition.IsVirtual)
		{
			throw new InvalidOperationException("Can not override non-virtual method '" + methodReference.FullName + "'.");
		}
		methodDefinition.IsReuseSlot = true;
		methodDefinition.DeclaringType = null;
		if (!methodDefinition.ReturnType.IsGenericParameter)
		{
			methodDefinition.ReturnType = type.Assembly.AssemblyDefinition.MainModule.ImportReference(methodDefinition.ReturnType);
		}
		foreach (ParameterDefinition item in methodDefinition.Parameters.Where((ParameterDefinition parameter) => !parameter.ParameterType.IsGenericParameter))
		{
			item.ParameterType = type.Assembly.AssemblyDefinition.MainModule.ImportReference(item.ParameterType);
		}
		type.TypeDefinition.Methods.Add(methodDefinition);
		return new GeneratedMethod
		{
			Name = methodDefinition.Name,
			Type = type,
			MethodDefinition = methodReference.Resolve()
		};
	}
}
