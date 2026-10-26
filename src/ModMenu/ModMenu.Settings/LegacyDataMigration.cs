using System.IO;
using System.Linq;

namespace ModMenu.Settings;

public static class LegacyDataMigration
{
	public static bool CopyIfEmpty(string oldRoot, string newRoot)
	{
		if (!HasFiles(oldRoot) || HasFiles(newRoot))
		{
			return false;
		}
		CopyDirectory(oldRoot, newRoot);
		return true;
	}

	private static bool HasFiles(string directory)
	{
		return Directory.Exists(directory) && Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories).Any();
	}

	private static void CopyDirectory(string source, string destination)
	{
		Directory.CreateDirectory(destination);
		foreach (string file in Directory.GetFiles(source))
		{
			File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), overwrite: false);
		}
		foreach (string directory in Directory.GetDirectories(source))
		{
			CopyDirectory(directory, Path.Combine(destination, Path.GetFileName(directory)));
		}
	}
}
