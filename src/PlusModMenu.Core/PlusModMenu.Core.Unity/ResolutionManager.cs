using System;
using System.Collections;
using UnityEngine;

namespace PlusModMenu.Core.Unity;

public class ResolutionManager
{
	private static readonly bool _active;

	public static event Action? OnResolutionChanged;

	static ResolutionManager()
	{
		_active = true;
		if (_active)
		{
			new TaskManager.SimpleTask(DetectResolutionChange).Start();
		}
	}

	private static IEnumerator DetectResolutionChange()
	{
		int width = Screen.width;
		int height = Screen.height;
		int height2 = height;
		int width2 = width;
		while (true)
		{
			if (Screen.width != width2 || Screen.height != height2)
			{
				OnResolutionChanged?.Invoke();
				height = Screen.width;
				width = Screen.height;
				height2 = width;
				width2 = height;
			}
			yield return null;
		}
	}
}
