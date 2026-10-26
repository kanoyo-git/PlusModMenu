using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PlusModMenu.Codegen;
using PlusModMenu.Core.Common;
using PlusModMenu.Core.Common.Extensions;
using PlusModMenu.Core.Common.Utils;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using MonoMod.Utils;
using ModMenu.Settings.Editors;

namespace ModMenu.BaldiPlusEngineGame.Services;

internal class ModMenuService : IModMenuService, IService
{
	private readonly HashSet<Type> _variableTypes = new HashSet<Type>();

	internal HashSet<IModifiers> _modifierInstances = new HashSet<IModifiers>();

	public IReadOnlyCollection<IModifiers> Modifiers => _modifierInstances;

	public TModifiers? Get<TModifiers>() where TModifiers : class, IModifiers
	{
		return _modifierInstances.FirstOrDefault((IModifiers x) => x is TModifiers) as TModifiers;
	}

	public object? Get(Type? modifiersType)
	{
		return _modifierInstances.FirstOrDefault((IModifiers x) => x.GetType().IsAssignableTo(modifiersType));
	}

	internal void Initialize()
	{
		GeneratedAssembly generatedAssembly = GeneratedAssembly.CreateNew("PlusModMenu");
		ModuleDefinition mainModule = generatedAssembly.AssemblyDefinition.MainModule;
		HashSet<Assembly> hashSet = new HashSet<Assembly>();
		foreach (IPlugin availablePlugin in Service.Get<PluginService>().AvailablePlugins)
		{
			hashSet.Add(availablePlugin.GetType().Assembly);
		}
		EditorInformation.UpdateEditorControlTable(hashSet);
		HashSet<Type> hashSet2 = new HashSet<Type>();
		foreach (Assembly item in hashSet)
		{
			IEnumerable<Type> suitableModifierTypes = GetSuitableModifierTypes(item);
			hashSet2.UnionWith(suitableModifierTypes);
		}
		foreach (Type item2 in hashSet2)
		{
			GeneratedType generatedType = null;
			try
			{
				generatedType = generatedAssembly.AddType(item2);
				IEnumerable<MemberInfo> source = GetSuitableMembers(item2).ToArray();
				foreach (PropertyInfo item3 in source.OfType<PropertyInfo>())
				{
					Type type = item3.ReturnType() ?? typeof(void);
					EditorInformation editorControl = EditorInformation.GetEditorControl(type, item2.Assembly, item3);
					if (editorControl != null)
					{
						GeneratedField generatedField = generatedType.AddField("<UI>" + item3.Name, Mono.Cecil.FieldAttributes.Private, editorControl.EditorType);
						GeneratedProperty generatedProperty = generatedType.AddProperty(item3);
						Collection<Instruction> instructions = generatedProperty.SetMethodDefinition.Body.Instructions;
						instructions.Clear();
						instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
						instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
						instructions.Add(Instruction.Create(OpCodes.Call, mainModule.ImportReference(item3.SetMethod)));
						instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
						instructions.Add(Instruction.Create(OpCodes.Ldfld, generatedField.FieldDefinition));
						instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
						instructions.Add(Instruction.Create(OpCodes.Call, mainModule.ImportReference(item3.GetMethod)));
						if (type.IsValueType)
						{
							instructions.Add(Instruction.Create(OpCodes.Box, mainModule.ImportReference(type)));
						}
						instructions.Add(Instruction.Create(OpCodes.Callvirt, mainModule.ImportReference(typeof(IEditor).GetMethod("SetValueWithoutNotify"))));
						instructions.Add(Instruction.Create(OpCodes.Ret));
						Collection<Instruction> collection = new Collection<Instruction>();
						collection.AddRange(PropertyCreationInstructions(mainModule, type, editorControl.EditorType, generatedField.FieldDefinition, generatedProperty));
						generatedField.InitializationInstructions = collection;
					}
				}
				foreach (MethodInfo item4 in source.OfType<MethodInfo>())
				{
					if (item4.ReturnType == typeof(void) && item4.GetParameters().Length < 1)
					{
						EditorInformation editorControl2 = EditorInformation.GetEditorControl(item4.ReturnType, item2.Assembly, item4);
						if (editorControl2 != null)
						{
							GeneratedField generatedField2 = generatedType.AddField("<UI>" + item4.Name, Mono.Cecil.FieldAttributes.Private, editorControl2.EditorType);
							Collection<Instruction> collection2 = new Collection<Instruction>();
							collection2.AddRange(MethodCreationInstructions(mainModule, editorControl2.EditorType, generatedField2, mainModule.ImportReference(item4)));
							generatedField2.InitializationInstructions = collection2;
						}
					}
				}
			}
			catch (Exception message)
			{
				generatedAssembly.AssemblyDefinition.MainModule.Types.Remove(generatedType?.TypeDefinition);
				Debugger.Error("The modifier type '" + item2.Name + "' caused an error, while trying to prepare it.");
				Debugger.Error(message);
			}
		}
		Assembly assembly = generatedAssembly.Save();
		_variableTypes.UnionWith(TypeUtils.GetLoadableTypes(assembly));
	}

	internal static IEnumerable<Instruction> InstantiateEditorControl(ModuleDefinition module, Type editorType, FieldDefinition uiFieldDefinition)
	{
		yield return Instruction.Create(OpCodes.Ldarg_0);
		yield return Instruction.Create(OpCodes.Newobj, module.ImportReference(editorType.GetConstructor(Array.Empty<Type>())));
		yield return Instruction.Create(OpCodes.Stfld, uiFieldDefinition);
	}

	internal static IEnumerable<Instruction> PropertyCreationInstructions(ModuleDefinition module, Type inputType, Type editorType, FieldDefinition uiFieldDefinition, GeneratedProperty generatedProperty)
	{
		foreach (Instruction item in InstantiateEditorControl(module, editorType, uiFieldDefinition))
		{
			yield return item;
		}
		VariableDefinition metadataVariable = new VariableDefinition(module.ImportReference(typeof(PropertyMetadata<>).MakeGenericType(inputType)));
		generatedProperty.Type.ConstructorDefinition.Body.Variables.Add(metadataVariable);
		yield return Instruction.Create(OpCodes.Ldarg_0);
		yield return Instruction.Create(OpCodes.Ldtoken, generatedProperty.Type.TypeDefinition);
		yield return Instruction.Create(OpCodes.Call, module.ImportReference(typeof(Type).GetMethod("GetTypeFromHandle")));
		yield return Instruction.Create(OpCodes.Ldstr, generatedProperty.Name);
		yield return Instruction.Create(OpCodes.Call, module.ImportReference(typeof(Type).GetMethod("GetProperty", new Type[1] { typeof(string) })));
		yield return Instruction.Create(OpCodes.Ldarg_0);
		yield return Instruction.Create(OpCodes.Ldftn, generatedProperty.GetMethodDefinition);
		yield return Instruction.Create(OpCodes.Newobj, module.ImportReference(typeof(Func<>).MakeGenericType(inputType).GetConstructor(new Type[2]
		{
			typeof(object),
			typeof(IntPtr)
		})));
		yield return Instruction.Create(OpCodes.Ldarg_0);
		yield return Instruction.Create(OpCodes.Ldftn, generatedProperty.SetMethodDefinition);
		yield return Instruction.Create(OpCodes.Newobj, module.ImportReference(typeof(Action<>).MakeGenericType(inputType).GetConstructor(new Type[2]
		{
			typeof(object),
			typeof(IntPtr)
		})));
		yield return Instruction.Create(OpCodes.Newobj, module.ImportReference(typeof(PropertyMetadata<>).MakeGenericType(inputType).GetConstructor(new Type[4]
		{
			typeof(object),
			typeof(MemberInfo),
			typeof(Func<>).MakeGenericType(inputType),
			typeof(Action<>).MakeGenericType(inputType)
		})));
		yield return Instruction.Create(OpCodes.Stloc, metadataVariable);
		yield return Instruction.Create(OpCodes.Ldarg_0);
		yield return Instruction.Create(OpCodes.Ldfld, uiFieldDefinition);
		yield return Instruction.Create(OpCodes.Ldloc, metadataVariable);
		yield return Instruction.Create(OpCodes.Callvirt, module.ImportReference(typeof(IEditor).GetMethod("Initialize")));
	}

	internal static IEnumerable<Instruction> MethodCreationInstructions(ModuleDefinition module, Type editorType, GeneratedField generatedField, MethodReference generatedMethod)
	{
		FieldDefinition uiFieldDefinition = generatedField.FieldDefinition;
		foreach (Instruction item in InstantiateEditorControl(module, editorType, uiFieldDefinition))
		{
			yield return item;
		}
		VariableDefinition metadataVariable = new VariableDefinition(module.ImportReference(typeof(MethodMetadata)));
		generatedField.Type.ConstructorDefinition.Body.Variables.Add(metadataVariable);
		yield return Instruction.Create(OpCodes.Ldarg_0);
		yield return Instruction.Create(OpCodes.Ldtoken, generatedMethod.DeclaringType);
		yield return Instruction.Create(OpCodes.Call, module.ImportReference(typeof(Type).GetMethod("GetTypeFromHandle")));
		yield return Instruction.Create(OpCodes.Ldstr, generatedMethod.Name);
		yield return Instruction.Create(OpCodes.Call, module.ImportReference(typeof(Type).GetMethod("GetMethod", new Type[1] { typeof(string) })));
		yield return Instruction.Create(OpCodes.Ldarg_0);
		yield return Instruction.Create(OpCodes.Ldftn, generatedMethod);
		yield return Instruction.Create(OpCodes.Newobj, module.ImportReference(typeof(Action).GetConstructor(new Type[2]
		{
			typeof(object),
			typeof(IntPtr)
		})));
		yield return Instruction.Create(OpCodes.Newobj, module.ImportReference(typeof(MethodMetadata).GetConstructor(new Type[3]
		{
			typeof(object),
			typeof(MemberInfo),
			typeof(Action)
		})));
		yield return Instruction.Create(OpCodes.Stloc, metadataVariable);
		yield return Instruction.Create(OpCodes.Ldarg_0);
		yield return Instruction.Create(OpCodes.Ldfld, uiFieldDefinition);
		yield return Instruction.Create(OpCodes.Ldloc, metadataVariable);
		yield return Instruction.Create(OpCodes.Callvirt, module.ImportReference(typeof(IEditor).GetMethod("Initialize")));
	}

	internal IEnumerable<Type> GetSuitableModifierTypes(Assembly assembly)
	{
		return from x in TypeUtils.GetLoadableTypes(assembly)
			where x.IsAssignableTo<IModifiers>()
			where !x.IsAbstract
			where !x.IsGenericType
			select x;
	}

	internal IEnumerable<MemberInfo> GetSuitableMembers(Type modifierType)
	{
		return from x in modifierType.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
			where !x.IsAbstract()
			where x.IsVirtual()
			select x;
	}

	internal void GenerateInstances()
	{
		_modifierInstances.Clear();
		foreach (Type variableType in _variableTypes)
		{
			_modifierInstances.Add((IModifiers)Activator.CreateInstance(variableType, nonPublic: true));
		}
	}
}
