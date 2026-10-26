using System;
using System.IO;
using System.Reflection;
using Mono.Cecil;

namespace PlusModMenu.Codegen;

public class GeneratedAssembly
{
	public AssemblyDefinition AssemblyDefinition { get; private init; }

	private GeneratedAssembly()
	{
	}

	public static GeneratedAssembly CreateNew(string name)
	{
		AssemblyNameDefinition assemblyNameDefinition = new AssemblyNameDefinition("<PMM.CODEGEN>" + name, new Version());
		GeneratedAssemblyResolver generatedAssemblyResolver = new GeneratedAssemblyResolver();
		generatedAssemblyResolver.AddSearchDirectory(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location));
		return new GeneratedAssembly
		{
			AssemblyDefinition = Mono.Cecil.AssemblyDefinition.CreateAssembly(assemblyNameDefinition, assemblyNameDefinition.Name + ".dll", new ModuleParameters
			{
				AssemblyResolver = generatedAssemblyResolver,
				Kind = ModuleKind.Dll
			})
		};
	}

	public Assembly Save()
	{
		using MemoryStream memoryStream = new MemoryStream();
		AssemblyDefinition.Write(memoryStream);
		return Assembly.Load(memoryStream.ToArray());
	}

	public GeneratedType AddType(Type originalType)
	{
		return GeneratedType.CreateExtension(AssemblyDefinition.MainModule.ImportReference(originalType), this);
	}
}
