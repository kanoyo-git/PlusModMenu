using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PlusModMenu.Core.Unity;

public class DataPaths
{
	private static string _dataPath;

	private static Dictionary<string, string> _dataPaths;

	public static bool IsPortable { get; private set; }

	public static void CheckPortableMode()
	{
		if (File.Exists(Application.dataPath + "/portable.txt"))
		{
			IsPortable = true;
			_dataPath = Application.dataPath;
		}
	}

	public static void AddPath(string key, string path, RelativeTo relativeTo)
	{
		AddPath(key, path, relativeTo switch
		{
			RelativeTo.Data => GetPath("Data"), 
			RelativeTo.Temp => GetPath("Temp"), 
			RelativeTo.Resources => GetPath("Resources"), 
			RelativeTo.Mod => GetPath("Mod Directory"), 
			_ => string.Empty, 
		});
	}

	public static void AddPath(string key, string path, string relativeTo)
	{
		if (!_dataPaths.ContainsKey(key))
		{
			string text = path;
			if (text[text.Length - 1] != '/')
			{
				path += "/";
			}
			string text2 = relativeTo + path;
			Directory.CreateDirectory(text2);
			_dataPaths.Add(key, text2);
		}
	}

	public static string GetPath(string key)
	{
		string text = _dataPaths[key];
		Directory.CreateDirectory(text);
		return text;
	}

	static DataPaths()
	{
		_dataPath = Application.persistentDataPath;
		_dataPaths = new Dictionary<string, string>();
		CheckPortableMode();
	}

	public static void Init(string author, string modName)
	{
		_dataPaths = new Dictionary<string, string>
		{
			{ "Data", _dataPath },
			{
				"Mod Directory",
				_dataPath + "/Mods/" + author + "/" + modName + "/"
			},
			{
				"Temp",
				Application.temporaryCachePath + "/Mods/" + author + "/" + modName + "/"
			},
			{
				"Resources",
				_dataPath + "/Mods/" + author + "/" + modName + "/resources/"
			}
		};
	}
}
