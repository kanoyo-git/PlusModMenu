using System;
using PlusModMenu.Core.Unity;
using ModMenu.Settings;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace ModMenu.Common.Serialization;

internal class JsonSerializationBinder : ISerializationBinder
{
	private readonly ISerializationBinder _binder = new DefaultSerializationBinder();

	private static readonly bool _merged = typeof(CoreLogger).Assembly == typeof(JsonSerializationBinder).Assembly;

	private static readonly string _current = typeof(JsonSerializationBinder).Assembly.FullName;

	public Type BindToType(string? assemblyName, string typeName)
	{
		try
		{
			return _merged
				? _binder.BindToType(LegacyAssemblies.MapTypeAssembly(assemblyName, _current), LegacyAssemblies.MapTypeName(typeName, _current))
				: _binder.BindToType(assemblyName, typeName);
		}
		catch (Exception ex)
		{
			CoreLogger.Log("At least one modifier type of the loaded Preset could not be found.", LogType.Warning);
			CoreLogger.Log(ex, LogType.Warning);
			throw new JsonSerializationTypeBoundException(ex);
		}
	}

	public void BindToName(Type serializedType, out string? assemblyName, out string? typeName)
	{
		_binder.BindToName(serializedType, out assemblyName, out typeName);
	}
}
