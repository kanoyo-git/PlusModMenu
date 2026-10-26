using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlusModMenu.Core.Unity;

public class InputHelper : MonoBehaviour
{
	private static readonly KeyCode[] _keyCodes;

	private readonly List<KeyCode> _keysDown = new List<KeyCode>();

	public static event Action<KeyCode>? KeyDownListener;

	public static event Action<KeyCode>? KeyUpListener;

	public static event Action<KeyCode>? KeyPressListener;

	static InputHelper()
	{
		_keyCodes = (from KeyCode x in Enum.GetValues(typeof(KeyCode))
			where x < KeyCode.Mouse0
			select x).ToArray();
		UnityEngine.Object.DontDestroyOnLoad(new GameObject("InputHelper", typeof(InputHelper)));
	}

	public static void _()
	{
	}

	public void OnEnable()
	{
		_keysDown.Clear();
	}

	public void Update()
	{
		if (Input.anyKeyDown)
		{
			for (int i = 0; i < _keyCodes.Length; i++)
			{
				KeyCode keyCode = _keyCodes[i];
				if (Input.GetKeyDown(keyCode))
				{
					_keysDown.Add(keyCode);
					KeyDownListener?.Invoke(keyCode);
				}
			}
		}
		if (_keysDown.Count <= 0)
		{
			return;
		}
		for (int j = 0; j < _keysDown.Count; j++)
		{
			KeyCode keyCode2 = _keysDown[j];
			KeyPressListener?.Invoke(keyCode2);
			if (Input.GetKeyUp(keyCode2))
			{
				_keysDown.RemoveAt(j);
				j--;
				KeyUpListener?.Invoke(keyCode2);
			}
		}
	}

	public static IEnumerable<KeyCode> KeyDown()
	{
		if (!Input.anyKeyDown)
		{
			yield break;
		}
		for (int i = 0; i < _keyCodes.Length; i++)
		{
			if (Input.GetKeyDown(_keyCodes[i]))
			{
				yield return _keyCodes[i];
			}
		}
	}

	public static IEnumerable<KeyCode> KeyPress()
	{
		if (!Input.anyKey)
		{
			yield break;
		}
		for (int i = 0; i < _keyCodes.Length; i++)
		{
			if (Input.GetKey(_keyCodes[i]))
			{
				yield return _keyCodes[i];
			}
		}
	}

	public static IEnumerable<KeyCode> KeyUp()
	{
		for (int i = 0; i < _keyCodes.Length; i++)
		{
			if (Input.GetKeyUp(_keyCodes[i]))
			{
				yield return _keyCodes[i];
			}
		}
	}
}
