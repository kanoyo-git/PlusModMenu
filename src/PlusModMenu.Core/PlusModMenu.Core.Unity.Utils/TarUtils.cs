using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.SharpZipLib.GZip;
using Unity.SharpZipLib.Tar;

namespace PlusModMenu.Core.Unity.Utils;

public static class TarUtils
{
	public static IEnumerable<(TarInputStream, TarEntry)> Entries(string path)
	{
		using Stream input = new FileStream(path, FileMode.Open, FileAccess.Read);
		using Stream decompressor = new GZipInputStream(input);
		using TarInputStream tarStream = new TarInputStream(decompressor, Encoding.UTF8);
		while (true)
		{
			TarEntry nextEntry = tarStream.GetNextEntry();
			if (nextEntry != null && nextEntry.Size > 0)
			{
				yield return (tarStream, nextEntry);
				continue;
			}
			break;
		}
	}
}
