using System;
using System.Linq;
using System.Reflection;
using PlusModMenu.Core.Mono.Cecil.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace PlusModMenu.Codegen;

public class GeneratedType
{
	public GeneratedAssembly Assembly { get; private init; }

	public TypeDefinition TypeDefinition { get; private init; }

	public MethodDefinition ConstructorDefinition { get; private init; }

	private GeneratedType()
	{
	}

	public static GeneratedType CreateExtension(TypeReference typeReference, GeneratedAssembly assembly)
	{
		TypeDefinition typeDefinition = typeReference.Resolve().Clone();
		typeDefinition.BaseType = typeReference;
		typeDefinition.IsSealed = true;
		typeDefinition.IsPublic = false;
		MethodDefinition method = typeDefinition.BaseType.Resolve().Methods.First((MethodDefinition m) => m != null && m.IsConstructor && !m.IsStatic && !m.HasParameters);
		MethodReference methodReference = assembly.AssemblyDefinition.MainModule.ImportReference(method);
		if (typeDefinition.BaseType.IsGenericInstance)
		{
			GenericInstanceType genericInstanceType = (GenericInstanceType)typeDefinition.BaseType;
			methodReference = methodReference.MakeGeneric(genericInstanceType.GenericArguments.ToArray());
		}
		MethodDefinition methodDefinition = new MethodDefinition(methodReference.Name, Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.HideBySig | Mono.Cecil.MethodAttributes.SpecialName | Mono.Cecil.MethodAttributes.RTSpecialName, assembly.AssemblyDefinition.MainModule.TypeSystem.Void);
		methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
		methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Call, methodReference));
		methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
		typeDefinition.Methods.Add(methodDefinition);
		assembly.AssemblyDefinition.MainModule.Types.Add(typeDefinition);
		return new GeneratedType
		{
			Assembly = assembly,
			TypeDefinition = typeDefinition,
			ConstructorDefinition = methodDefinition
		};
	}

	public GeneratedProperty AddProperty(PropertyInfo originalProperty)
	{
		GeneratedMethod generatedMethod = AddMethodOverride(originalProperty.GetMethod);
		Collection<Instruction> instructions = generatedMethod.MethodDefinition.Body.Instructions;
		instructions.Clear();
		instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
		instructions.Add(Instruction.Create(OpCodes.Call, Assembly.AssemblyDefinition.MainModule.ImportReference(originalProperty.GetMethod)));
		instructions.Add(Instruction.Create(OpCodes.Ret));
		GeneratedMethod generatedMethod2 = AddMethodOverride(originalProperty.SetMethod);
		Collection<Instruction> instructions2 = generatedMethod2.MethodDefinition.Body.Instructions;
		instructions2.Clear();
		instructions2.Add(Instruction.Create(OpCodes.Ldarg_0));
		instructions2.Add(Instruction.Create(OpCodes.Ldarg_1));
		instructions2.Add(Instruction.Create(OpCodes.Call, Assembly.AssemblyDefinition.MainModule.ImportReference(originalProperty.SetMethod)));
		instructions2.Add(Instruction.Create(OpCodes.Ret));
		return GeneratedProperty.CreateNew(originalProperty.Name, generatedMethod.MethodDefinition, generatedMethod2.MethodDefinition, this);
	}

	public GeneratedMethod AddMethod(string name, Mono.Cecil.MethodAttributes methodAttributes, Type returnType)
	{
		TypeReference returnType2 = Assembly.AssemblyDefinition.MainModule.ImportReference(returnType);
		return GeneratedMethod.CreateNew(name, methodAttributes, returnType2, this);
	}

	public GeneratedMethod AddMethodOverride(MethodInfo originalMethod)
	{
		return GeneratedMethod.CreateOverride(Assembly.AssemblyDefinition.MainModule.ImportReference(originalMethod), this);
	}

	public GeneratedField AddField(string name, Mono.Cecil.FieldAttributes fieldAttributes, Type fieldType)
	{
		TypeReference fieldType2 = Assembly.AssemblyDefinition.MainModule.ImportReference(fieldType);
		return GeneratedField.CreateNew(name, fieldAttributes, fieldType2, this);
	}
}
