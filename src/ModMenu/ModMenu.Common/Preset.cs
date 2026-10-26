using System;
using System.IO;
using System.Linq;
using System.Text;
using PlusModMenu.Core.Unity;
using PlusModMenu.Core.Unity.Utils;
using ModMenu.BaldiPlusEngineGame.Services;
using ModMenu.Settings;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using Newtonsoft.Json;
using Unity.SharpZipLib.Core;
using Unity.SharpZipLib.GZip;
using Unity.SharpZipLib.Tar;
using UnityEngine;

namespace ModMenu.Common;

public struct Preset
{
	public enum PresetType
	{
		Invalid = -1,
		Alpha
	}

	public readonly struct Metadata
	{
		public string Name { get; init; } = "<Invalid>";

		public PresetType PresetType { get; init; } = PresetType.Invalid;

		public Metadata()
		{
		}
	}

	private const PresetType REQUIRED_TYPE = PresetType.Alpha;

	private readonly string _filePath;

	private readonly Metadata _metadata;

	public string Name => _metadata.Name;

	public PresetType Type => _metadata.PresetType;

	public Preset(string path)
	{
		_metadata = default;
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
			CoreLogger.Log("The preset located at \"" + path + "\" could not be read.", LogType.Warning);
			CoreLogger.Log(msg, LogType.Error);
		}
	}

	public static Preset[] GetPresetsFromDir(string path)
	{
		return (from t in new DirectoryInfo(path).GetFiles("*.fmp")
			select new Preset(t.FullName) into x
			where x.Type == PresetType.Alpha
			select x).ToArray();
	}

	public static void Create<T>(string name, T obj)
	{
		name = CreateValidPresetName(name);
		Metadata metadata = new Metadata
		{
			Name = name,
			PresetType = PresetType.Alpha
		};
		using (FileStream baseOutputStream = new FileStream(Path.Combine(DataPaths.GetPath("Presets"), name + ".fmp"), FileMode.Create, FileAccess.Write))
		{
			using GZipOutputStream outputStream = new GZipOutputStream(baseOutputStream);
			using TarOutputStream tarOutputStream = new TarOutputStream(outputStream, Encoding.UTF8);
			tarOutputStream.IsStreamOwner = false;
			AddTarContent(tarOutputStream, "meta", JsonConvert.SerializeObject(metadata));
			AddTarContent(tarOutputStream, "data", PresetSerializer.Serialize(obj));
			CoreLogger.Log("Preset saved successfully");
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

	private static string CreateValidPresetName(string input)
	{
		return SaveFileNames.Unique(DataPaths.GetPath("Presets"), SaveFileNames.Clean(input, "A preset"), ".fmp");
	}

	public void Load()
	{
		try
		{
			foreach (var (stream, tarEntry) in TarUtils.Entries(_filePath))
			{
				if (string.Equals(tarEntry.Name, "data", StringComparison.Ordinal))
				{
					byte[] array = new byte[tarEntry.Size];
					StreamUtils.ReadFully(stream, array);
					PresetSerializer.Populate(Encoding.UTF8.GetString(array), ((ModMenuService)Service.Get<IModMenuService>())._modifierInstances);
					break;
				}
			}
		}
		catch (Exception msg)
		{
			CoreLogger.Log("The preset located at \"" + _filePath + "\" caused an error, while trying to apply its values.", LogType.Error);
			CoreLogger.Log(msg, LogType.Error);
		}
	}

	public void Delete()
	{
		if (File.Exists(_filePath))
		{
			File.Delete(_filePath);
		}
	}
}
