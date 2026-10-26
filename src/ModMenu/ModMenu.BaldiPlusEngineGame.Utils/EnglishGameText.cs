using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Utils;

internal static class EnglishGameText
{
	private static Dictionary<string, string>? _text;

	public static string Get(string key)
	{
		if (_text == null)
		{
			_text = Load();
		}
		return _text.TryGetValue(key, out string? value) ? value : Singleton<LocalizationManager>.Instance.GetLocalizedText(key);
	}

	private static Dictionary<string, string> Load()
	{
		Dictionary<string, string> text = new Dictionary<string, string>();
		try
		{
			LocalizationData data = JsonUtility.FromJson<LocalizationData>(File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Subtitles_En.json")));
			foreach (LocalizationItem item in data.items)
			{
				text[item.key] = item.value;
			}
		}
		catch (Exception e)
		{
			Debug.LogWarning("Plus Mod Menu: couldn't read the game's English text: " + e.Message);
		}
		return text;
	}
}
