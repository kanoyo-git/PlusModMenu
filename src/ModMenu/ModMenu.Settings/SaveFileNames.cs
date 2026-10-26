using System.IO;
using System.Linq;

namespace ModMenu.Settings;

public static class SaveFileNames
{
	private const string Forbidden = "\\/:*?\"<>|";

	public static string Clean(string input, string fallback)
	{
		var kept = new string((input ?? string.Empty).Where(c => !char.IsControl(c) && Forbidden.IndexOf(c) < 0).ToArray());
		kept = kept.Trim().Trim('.').Trim();
		return kept.Length == 0 ? fallback : kept;
	}

	public static string Unique(string directory, string name, string extension)
	{
		var candidate = name;
		for (var i = 2; File.Exists(Path.Combine(directory, candidate + extension)); i++)
		{
			candidate = $"{name} ({i})";
		}
		return candidate;
	}
}
