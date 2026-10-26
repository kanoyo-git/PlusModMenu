using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace PlusModMenu.Codegen;

public class GeneratedAssemblyResolver : BaseAssemblyResolver
{
	private readonly IDictionary<string, AssemblyDefinition> _cache = new Dictionary<string, AssemblyDefinition>(StringComparer.Ordinal);

	public override AssemblyDefinition Resolve(AssemblyNameReference name)
	{
		if (_cache.TryGetValue(name.FullName, out AssemblyDefinition value))
		{
			return value;
		}
		return _cache[name.FullName] = base.Resolve(name);
	}

	protected void RegisterAssembly(AssemblyDefinition assembly)
	{
		if (assembly == null)
		{
			throw new ArgumentNullException("assembly");
		}
		string fullName = assembly.Name.FullName;
		if (!_cache.ContainsKey(fullName))
		{
			_cache[fullName] = assembly;
		}
	}

	protected override void Dispose(bool disposing)
	{
		foreach (AssemblyDefinition value in _cache.Values)
		{
			value.Dispose();
		}
		_cache.Clear();
		base.Dispose(disposing);
	}
}
