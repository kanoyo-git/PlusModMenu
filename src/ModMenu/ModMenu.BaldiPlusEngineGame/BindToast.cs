using System.Collections.Generic;
using ModMenu.Settings;
using PlusModMenu.Api.Settings;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame;

internal class BindToast : MonoBehaviour
{
	private const float ShowTime = 2f;

	private const float FadeTime = 0.5f;

	private static BindToast? _instance;

	private readonly List<(string text, float until)> _lines = new List<(string, float)>();

	private GUIStyle? _style;

	public static IReadOnlyList<string> Lines
	{
		get
		{
			var lines = new List<string>();
			if (_instance != null)
			{
				foreach (var (text, until) in _instance._lines)
				{
					if (until > Time.unscaledTime)
					{
						lines.Add(text);
					}
				}
			}
			return lines;
		}
	}

	public static void Show(SettingEntry entry)
	{
		Show(Describe(entry));
	}

	public static void Show(string text)
	{
		if (_instance == null)
		{
			_instance = new GameObject("ModMenuBindToast").AddComponent<BindToast>();
			DontDestroyOnLoad(_instance.gameObject);
		}
		_instance._lines.RemoveAll(line => line.text == text || line.until <= Time.unscaledTime);
		_instance._lines.Add((text, Time.unscaledTime + ShowTime));
		if (_instance._lines.Count > 4)
		{
			_instance._lines.RemoveAt(0);
		}
	}

	internal static string Describe(SettingEntry entry)
	{
		var label = entry.Label.Value;
		switch (entry.Kind)
		{
		case SettingKind.Toggle:
			return label + ": " + (entry.GetValue() is true ? Strings.TOGGLE_ON : Strings.TOGGLE_OFF);
		case SettingKind.Choice:
			var options = entry.Options();
			var index = entry.GetValue() is int i ? i : 0;
			return index >= 0 && index < options.Count ? label + ": " + options[index].Value : label;
		default:
			return label;
		}
	}

	private void OnGUI()
	{
		var now = Time.unscaledTime;
		_lines.RemoveAll(line => line.until <= now);
		if (_lines.Count == 0)
		{
			return;
		}
		_style ??= new GUIStyle(GUI.skin.box) { fontSize = 18, alignment = TextAnchor.MiddleCenter, wordWrap = false };
		var y = 12f;
		var color = GUI.color;
		foreach (var (text, until) in _lines)
		{
			var content = new GUIContent(text);
			var size = _style.CalcSize(content);
			var width = size.x + 24f;
			GUI.color = new Color(1f, 1f, 1f, Mathf.Clamp01((until - now) / FadeTime));
			GUI.Box(new Rect((Screen.width - width) / 2f, y, width, size.y + 8f), content, _style);
			y += size.y + 12f;
		}
		GUI.color = color;
	}
}
