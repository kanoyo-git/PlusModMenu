using System.IO;
using System.Reflection;
using PlusModMenu.Core.Common;
using Unity.SharpZipLib.GZip;

namespace PlusModMenu.Core.Unity;

public class ContentPack
{
	private static TarFile PrepareTarFile(string resourceName, Assembly assembly)
	{
		MemoryStream memoryStream = new MemoryStream();
		using (GZipInputStream input = new GZipInputStream(PlusModMenu.Core.Common.ResourceManager.GetStream(resourceName, assembly)))
		{
			CopyTo(input, memoryStream);
		}
		memoryStream.Position = 0L;
		return new TarFile(memoryStream);
	}

	public static void UnpackToMemory(string resourceName)
	{
		using TarFile tarFile = PrepareTarFile(resourceName, Assembly.GetCallingAssembly());
		foreach (TarFile.FileInfo item in tarFile)
		{
			PlusModMenu.Core.Common.ResourceManager.AddMemoryContent(item.Name, item.Data);
		}
	}

	public static void UnpackToDisk(string resourceName, bool overwrite)
	{
		using TarFile tarFile = PrepareTarFile(resourceName, Assembly.GetCallingAssembly());
		foreach (TarFile.FileInfo item in tarFile)
		{
			string path = Path.Combine(DataPaths.GetPath("Resources"), item.Name);
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			if (overwrite || !File.Exists(path))
			{
				using FileStream fileStream = File.Open(path, FileMode.Create, FileAccess.Write);
				fileStream.Write(item.Data, 0, item.Data.Length);
			}
		}
	}

	private static void CopyTo(Stream input, Stream output)
	{
		byte[] array = new byte[16384];
		int count;
		while ((count = input.Read(array, 0, array.Length)) > 0)
		{
			output.Write(array, 0, count);
		}
	}

	public static void RemoveContent()
	{
		Directory.Delete(DataPaths.GetPath("Resources"), recursive: true);
	}
}
