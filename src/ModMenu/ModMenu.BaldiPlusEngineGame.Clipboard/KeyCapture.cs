using System;
using System.Collections.Generic;
using System.Linq;
using PlusModMenu.Api.Settings;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Clipboard;

internal static class KeyCapture
{
	private const float Timeout = 5f;

	private static readonly KeyCode[] Keys = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().Where(x => x > KeyCode.None && x < KeyCode.Mouse0).Distinct().ToArray();

	private static readonly KeyCode[] KeysAndMouse = Keys.Concat(new[] { KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Mouse2, KeyCode.Mouse3, KeyCode.Mouse4 }).ToArray();

	private static readonly List<(ISettingHandle Handle, string Label, Func<KeyCode> Get)> _buttons = new List<(ISettingHandle, string, Func<KeyCode>)>();

	private static KeyCode[] _keys = Keys;

	private static int _startFrame;

	private static bool _passEscape;

	private static int? _mouseFinish;

	private static Action<int> _tick;

	private static Action<KeyCode?> _done;

	private static float _left;

	private static int _shown;

	public static bool IsActive => _done != null;

	public static void Start(Action<int> tick, Action<KeyCode?> done, bool mouse = false, bool passEscape = false)
	{
		if (_mouseFinish is int button)
		{
			_mouseFinish = null;
			if (Input.GetMouseButtonUp(button))
			{
				return;
			}
		}
		Cancel();
		_keys = mouse ? KeysAndMouse : Keys;
		_passEscape = passEscape;
		_startFrame = Time.frameCount;
		_tick = tick;
		_done = done;
		_left = Timeout;
		_shown = -1;
		Tick();
	}

	public static void Cancel()
	{
		Finish(null);
	}

	public static void Update()
	{
		if (!IsActive)
		{
			return;
		}
		if (Input.anyKeyDown && Time.frameCount != _startFrame)
		{
			foreach (var key in _keys)
			{
				if (Input.GetKeyDown(key))
				{
					if (key >= KeyCode.Mouse0)
					{
						_mouseFinish = key - KeyCode.Mouse0;
					}
					Finish(key == KeyCode.Escape && !_passEscape ? null : key);
					return;
				}
			}
		}
		_left -= Time.unscaledDeltaTime;
		if (_left <= 0f)
		{
			Finish(null);
			return;
		}
		Tick();
	}

	private static void Tick()
	{
		var seconds = Mathf.CeilToInt(_left);
		if (seconds != _shown)
		{
			_shown = seconds;
			_tick?.Invoke(seconds);
		}
	}

	private static void Finish(KeyCode? key)
	{
		var done = _done;
		_done = null;
		_tick = null;
		done?.Invoke(key);
	}

	public static ISettingHandle AddButton(ISettingsGroup group, string label, Func<KeyCode> get, Action<KeyCode> set, bool mouse = false)
	{
		ISettingHandle button = null;
		button = group.AddButton(UIText.Resource<Strings>(label, get()), () => Start(seconds =>
		{
			button.Label = UIText.Resource<Strings>("WAITING_FOR_INPUT__TIME", seconds);
		}, key =>
		{
			if (key.HasValue)
			{
				set(key.Value);
			}
			foreach (var other in _buttons)
			{
				if (other.Label == label)
				{
					other.Handle.Label = UIText.Resource<Strings>(label, other.Get());
				}
			}
		}, mouse));
		_buttons.Add((button, label, get));
		return button;
	}
}
