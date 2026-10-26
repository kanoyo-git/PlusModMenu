using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace PlusModMenu.Codegen;

public class GeneratedField
{
	private Collection<Instruction> _initializationInstructions = new Collection<Instruction>();

	public GeneratedType Type { get; private init; }

	public FieldDefinition FieldDefinition { get; private init; }

	public Collection<Instruction> InitializationInstructions
	{
		get
		{
			return _initializationInstructions;
		}
		set
		{
			foreach (Instruction initializationInstruction in _initializationInstructions)
			{
				Type.ConstructorDefinition.Body.Instructions.Remove(initializationInstruction);
			}
			Instruction instruction = value.LastOrDefault((Instruction x) => x.OpCode == OpCodes.Ret);
			if (instruction != null)
			{
				value.Remove(instruction);
			}
			foreach (Instruction item in value)
			{
				Type.ConstructorDefinition.Body.Instructions.Insert(Type.ConstructorDefinition.Body.Instructions.Count - 1, item);
			}
			_initializationInstructions = value;
		}
	}

	private GeneratedField()
	{
	}

	public static GeneratedField CreateNew(string name, FieldAttributes fieldAttributes, TypeReference fieldType, GeneratedType type)
	{
		FieldDefinition fieldDefinition = new FieldDefinition(name, fieldAttributes, fieldType);
		type.TypeDefinition.Fields.Add(fieldDefinition);
		return new GeneratedField
		{
			Type = type,
			FieldDefinition = fieldDefinition
		};
	}
}
