using UnityEngine;

namespace ModMenu.Common;

internal class FrameCounter : MonoBehaviour
{
	private float _timeCounter;

	private float _lastFramerate;

	private int _frameCounter;

	private static FrameCounter? _instance;

	public static FrameCounter Instance
	{
		get
		{
			if (_instance != null)
			{
				return _instance;
			}
			FrameCounter frameCounter = new GameObject("FrameCounter").AddComponent<FrameCounter>();
			Object.DontDestroyOnLoad(frameCounter);
			_instance = frameCounter;
			return frameCounter;
		}
	}

	private void Awake()
	{
		enabled = false;
	}

	private void OnGUI()
	{
		CalcFramerate();
		GUI.Label(new Rect(10f, 10f, 100f, 20f), _lastFramerate.ToString("#"));
	}

	private void CalcFramerate()
	{
		if (_timeCounter < 1f)
		{
			_timeCounter += Time.unscaledDeltaTime;
			_frameCounter++;
		}
		else
		{
			_lastFramerate = (float)_frameCounter / _timeCounter;
			_frameCounter = 0;
			_timeCounter = 0f;
		}
	}
}
