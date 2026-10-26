using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ModMenu.Settings;

public static class LegacyAssemblies
{
	private static readonly string[] _oldMod = { "ModMenu", "Bepity", "Daelite", "FaDe", "ModMenuAPI" };

	private static readonly string[] _oldLibraries = { "Newtonsoft.Json", "Unity.SharpZipLib" };

	private static readonly string[] _oldAssemblies = _oldMod.Concat(_oldLibraries).ToArray();

	private static readonly (string from, string to)[] _namespaces =
	{
		("ModMenuAPI", "PlusModMenu.Api"),
		("FaDe", "PlusModMenu.Core"),
		("Bepity", "PlusModMenu.Hooks"),
		("Daelite", "PlusModMenu.Codegen"),
	};

	private static readonly Regex _assemblyInGeneric = new Regex(@",\s*(" + string.Join("|", _oldAssemblies.Select(Regex.Escape)) + @")(\s*,\s*(Version|Culture|PublicKeyToken)=[^,\]]*)*(?=\s*\])");

	public static string? MapTypeAssembly(string? saved, string current)
	{
		if (saved == null)
		{
			return null;
		}
		string name = saved.Split(',')[0].Trim();
		return _oldAssemblies.Contains(name, StringComparer.OrdinalIgnoreCase) ? current : saved;
	}

	public static string MapTypeName(string saved, string current)
	{
		string name = saved;
		foreach (var (from, to) in _namespaces)
		{
			name = Regex.Replace(name, @"(?<![\w.])" + Regex.Escape(from) + @"\.", to + ".");
		}
		return _assemblyInGeneric.Replace(name, ", " + current);
	}

	public static IEnumerable<string> Leftovers(IEnumerable<string> relativePaths, string self)
	{
		string me = Normalize(self);
		var files = relativePaths.Select(Normalize).Where(x => x != me).ToList();
		var oldDlls = files.Where(f => _oldMod.Any(n => FileName(f).Equals(n + ".dll", StringComparison.OrdinalIgnoreCase))).ToList();
		var oldDirs = new HashSet<string>(oldDlls.Select(Directory));
		var companions = files.Where(f => oldDirs.Contains(Directory(f)) && _oldLibraries.Any(n => FileName(f).Equals(n + ".dll", StringComparison.OrdinalIgnoreCase) || FileName(f).Equals(n + ".License.txt", StringComparison.OrdinalIgnoreCase)));
		return oldDlls.Concat(companions).Distinct().OrderBy(f => f, StringComparer.Ordinal).ToList();
	}

	private static string Normalize(string path) => path.Replace('\\', '/');

	private static string FileName(string path) => path.Substring(path.LastIndexOf('/') + 1);

	private static string Directory(string path) => path.LastIndexOf('/') < 0 ? string.Empty : path.Substring(0, path.LastIndexOf('/'));
}
