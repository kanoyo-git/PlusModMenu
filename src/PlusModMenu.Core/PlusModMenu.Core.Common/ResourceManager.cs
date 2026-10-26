using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using PlusModMenu.Core.Unity;
using UnityEngine;

namespace PlusModMenu.Core.Common;

public class ResourceManager
{
	protected static readonly Dictionary<string, byte[]> _memoryContent = new Dictionary<string, byte[]>();

	public static string GetString(string resourceName)
	{
		return GetString(resourceName, Assembly.GetCallingAssembly());
	}

	public static string GetString(string resourceName, Assembly assembly)
	{
		byte[] bytes = GetBytes(resourceName, assembly);
		return Encoding.UTF8.GetString(bytes);
	}

	public static byte[] GetBytes(string resourceName)
	{
		return GetBytes(resourceName, Assembly.GetCallingAssembly());
	}

	public static byte[] GetBytes(string resourceName, Assembly assembly)
	{
		using Stream stream = GetStream(resourceName, assembly);
		using MemoryStream memoryStream = new MemoryStream();
		byte[] array = new byte[16384];
		int count;
		while ((count = stream.Read(array, 0, array.Length)) > 0)
		{
			memoryStream.Write(array, 0, count);
		}
		return memoryStream.ToArray();
	}

	public static Stream GetStream(string resourceName)
	{
		return GetStream(resourceName, Assembly.GetCallingAssembly());
	}

	public static Stream GetStream(string resourceName, Assembly assembly)
	{
		Stream manifestResourceStream = assembly.GetManifestResourceStream(resourceName);
		if (manifestResourceStream != null)
		{
			return manifestResourceStream;
		}
		if (_memoryContent.ContainsKey(resourceName))
		{
			return new MemoryStream(_memoryContent[resourceName]);
		}
		if (!File.Exists(DataPaths.GetPath("Resources") + resourceName))
		{
			CoreLogger.Log("The requested resource \"" + resourceName + "\" could not be found.", LogType.Error);
			return new MemoryStream();
		}
		return File.OpenRead(DataPaths.GetPath("Resources") + resourceName);
	}

	public static void AddMemoryContent(string resourceName, byte[] content)
	{
		if (!_memoryContent.ContainsKey(resourceName))
		{
			_memoryContent.Add(resourceName, content);
		}
	}

	public static string[] GetEmbeddedResourceNames()
	{
		return GetEmbeddedResourceNames(Assembly.GetCallingAssembly());
	}

	public static string[] GetEmbeddedResourceNames(Assembly assembly)
	{
		return assembly.GetManifestResourceNames();
	}

	public static string[] GetEmbeddedResourcesFromDirectory(string path)
	{
		return GetEmbeddedResourcesFromDirectory(path, Assembly.GetCallingAssembly());
	}

	public static string[] GetEmbeddedResourcesFromDirectory(string path, Assembly assembly)
	{
		return (from x in assembly.GetManifestResourceNames()
			where x.StartsWith(path)
			select x).ToArray();
	}
}
