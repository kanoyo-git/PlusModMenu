using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PlusModMenu.Core.Unity;
using PlusModMenu.Core.Unity.Utils;
using ModMenu.Common;
using ModMenu.Settings;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.SharpZipLib.Core;
using Unity.SharpZipLib.GZip;
using Unity.SharpZipLib.Tar;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame;

public class Hotkey
{
	public struct Metadata
	{
		public string Name { get; set; } = "<Invalid>";

		public KeyCode Key { get; set; } = KeyCode.None;

		public bool Enabled { get; set; } = false;

		public Metadata()
		{
		}
	}

	private string _filePath;

	private Metadata _metadata;

	private string[] _states = Array.Empty<string>();

	private int _index;

	public string Name => _metadata.Name;

	public KeyCode Key
	{
		get
		{
			return _metadata.Key;
		}
		set
		{
			_metadata.Key = value;
			Update(this);
		}
	}

	public bool Enabled
	{
		get
		{
			return _metadata.Enabled;
		}
		set
		{
			_metadata.Enabled = value;
			Update(this);
		}
	}

	private Hotkey()
	{
	}

	public Hotkey(string path)
	{
		_filePath = path;
		try
		{
			foreach (var (stream, tarEntry) in TarUtils.Entries(path))
			{
				if (string.Equals(tarEntry.Name, "meta", StringComparison.Ordinal))
				{
					byte[] array = new byte[tarEntry.Size];
					StreamUtils.ReadFully(stream, array);
					_metadata = JsonConvert.DeserializeObject<Metadata>(Encoding.UTF8.GetString(array));
					break;
				}
			}
		}
		catch (Exception msg)
		{
			_metadata = default;
			CoreLogger.Log("The hotkey located at \"" + path + "\" could not be read.", LogType.Warning);
			CoreLogger.Log(msg, LogType.Error);
		}
	}

	public static Hotkey[] GetHotkeysFromDir(string path)
	{
		return (from t in new DirectoryInfo(path).GetFiles("*.hotkey")
			select new Hotkey(t.FullName)).ToArray();
	}

	private static void Update(Hotkey hotkey)
	{
		if (hotkey._states.Length == 0 && File.Exists(hotkey._filePath))
		{
			hotkey.Load();
		}
		using (FileStream baseOutputStream = new FileStream(hotkey._filePath, FileMode.Create, FileAccess.Write))
		{
			using GZipOutputStream outputStream = new GZipOutputStream(baseOutputStream);
			using TarOutputStream tarOutputStream = new TarOutputStream(outputStream, Encoding.UTF8);
			tarOutputStream.IsStreamOwner = false;
			AddTarContent(tarOutputStream, "meta", JsonConvert.SerializeObject(hotkey._metadata));
			for (int i = 0; i < hotkey._states.Length; i++)
			{
				AddTarContent(tarOutputStream, $"state{i}", hotkey._states[i]);
			}
			CoreLogger.Log("Hotkey saved successfully");
		}
		static void AddTarContent(TarOutputStream tarOutputStream2, string entryName, string content)
		{
			AddTarContentBytes(tarOutputStream2, entryName, Encoding.UTF8.GetBytes(content));
		}
		static void AddTarContentBytes(TarOutputStream tarOutputStream2, string entryName, byte[] content)
		{
			TarEntry tarEntry = TarEntry.CreateTarEntry(entryName);
			tarEntry.Size = content.Length;
			tarOutputStream2.PutNextEntry(tarEntry);
			tarOutputStream2.Write(content, 0, content.Length);
			tarOutputStream2.CloseEntry();
		}
	}

	public static void Create(string name, IEnumerable<JToken> states)
	{
		name = SaveFileNames.Unique(DataPaths.GetPath("Hotkeys"), SaveFileNames.Clean(name, "A hotkey"), ".hotkey");
		Update(new Hotkey
		{
			_metadata = 
			{
				Enabled = true,
				Key = KeyCode.None,
				Name = name
			},
			_filePath = Path.Combine(DataPaths.GetPath("Hotkeys"), name + ".hotkey"),
			_states = states.Select((JToken x) => x.ToString(Formatting.None)).ToArray()
		});
	}

	public void Load()
	{
		if (_states.Length != 0)
		{
			return;
		}
		try
		{
			List<string> list = new List<string>();
			foreach (var (stream, tarEntry) in TarUtils.Entries(_filePath))
			{
				if (tarEntry.Name.StartsWith("state", StringComparison.Ordinal))
				{
					byte[] array = new byte[tarEntry.Size];
					StreamUtils.ReadFully(stream, array);
					list.Add(Encoding.UTF8.GetString(array));
				}
			}
			_states = list.ToArray();
		}
		catch (Exception msg)
		{
			CoreLogger.Log("The hotkey located at \"" + _filePath + "\" caused an error, while trying to load it.", LogType.Error);
			CoreLogger.Log(msg, LogType.Error);
		}
	}

	public void Run()
	{
		if (Enabled)
		{
			if (_states.Length == 0)
			{
				Load();
			}
			PresetSerializer.Populate(_states[_index++], Service.Get<IModMenuService>().Modifiers);
			if (_index > _states.Length - 1)
			{
				_index = 0;
			}
		}
	}

	public void Delete()
	{
		if (File.Exists(_filePath))
		{
			File.Delete(_filePath);
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < _states.Length; i++)
		{
			stringBuilder.Append("State ");
			stringBuilder.Append(i);
			stringBuilder.AppendLine(":");
			stringBuilder.AppendLine(string.Empty);
			JToken jToken = JToken.Parse(_states[i]);
			if (i < _states.Length - 1)
			{
				stringBuilder.AppendLine(jToken.ToString(Formatting.Indented));
				stringBuilder.AppendLine(string.Empty);
			}
			else
			{
				stringBuilder.Append(jToken.ToString(Formatting.Indented));
			}
		}
		return stringBuilder.ToString();
	}
}
