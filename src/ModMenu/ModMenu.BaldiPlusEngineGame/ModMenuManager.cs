using System;
using System.Collections;
using System.Linq;
using PlusModMenu.Core.Common.Utils;
using PlusModMenu.Core.Unity;
using ModMenu.BaldiPlusEngineGame.Clipboard;
using ModMenu.BaldiPlusEngineGame.Internal.Menus;
using ModMenu.BaldiPlusEngineGame.Services;
using ModMenu.BaldiPlusEngineGame.Settings;
using ModMenu.Common.Serialization;
using PlusModMenu.Api;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces.Services;
using PlusModMenu.Api.Settings;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ModMenu.BaldiPlusEngineGame;

internal class ModMenuManager : MonoBehaviour
{
	private class PauseManager
	{
		private bool _paused;

		public void Pause(bool state)
		{
			if (state)
			{
				StateManager.Store();
				AudioListener.pause = true;
				Time.timeScale = 0f;
				Cursor.visible = false;
				StateManager.LockStates = true;
				_paused = true;
			}
			else if (_paused)
			{
				StateManager.LockStates = false;
				StateManager.Restore();
				_paused = false;
			}
		}
	}

	internal class StateManager
	{
		public static bool LockStates { get; set; }

		public static float? TimeScale { get; set; }

		public static CursorLockMode? CursorLockState { get; set; }

		public static bool? CursorVisible { get; set; }

		public static bool? AudioListenerPause { get; set; }

		public static void Store()
		{
			TimeScale = Time.timeScale;
			CursorLockState = Cursor.lockState;
			CursorVisible = Cursor.visible;
			AudioListenerPause = AudioListener.pause;
		}

		public static void Restore()
		{
			if (TimeScale.HasValue)
			{
				Time.timeScale = TimeScale.Value;
				TimeScale = null;
			}
			if (CursorLockState.HasValue)
			{
				Cursor.lockState = CursorLockState.Value;
				CursorLockState = null;
			}
			if (CursorVisible.HasValue)
			{
				Cursor.visible = CursorVisible.Value;
				CursorVisible = null;
			}
			if (AudioListenerPause.HasValue)
			{
				AudioListener.pause = AudioListenerPause.Value;
				AudioListenerPause = null;
			}
		}
	}

	private static ModMenuManager? _instance;

	private static string _resetPoint = "{}";

	private bool _wasLocked;

	private readonly IEventService _eventService = Service.Get<IEventService>();

	private readonly PauseManager _pauseManager = new PauseManager();

	public static Font DefaultFont { get; } = Resources.GetBuiltinResource<Font>("Arial.ttf");

	public static RandomEvent[] Events { get; private set; } = Array.Empty<RandomEvent>();

	public static void Create()
	{
		if (!(_instance != null))
		{
			GameObject gameObject = new GameObject("ModMenuManager");
			_instance = gameObject.AddComponent<ModMenuManager>();
			ClipboardMenu menu = ClipboardMenu.Create(gameObject.transform);
			menu.Opened += _instance.OnMenuOpened;
			menu.Closed += _instance.OnMenuClosed;
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			Presets.Init();
			Teleporter.Init();
			CreateResetPoint();
		}
	}

	private void Start()
	{
		SceneManager.sceneLoaded += FindGameSceneObjects;
		static void FindGameSceneObjects(Scene scene, LoadSceneMode _)
		{
			if (IsAnyGameScene(scene))
			{
				Events = Resources.FindObjectsOfTypeAll<RandomEvent>().DistinctBy((RandomEvent x) => x.GetType()).ToArray();
				SceneManager.sceneLoaded -= FindGameSceneObjects;
			}
		}
	}

	private void OnMenuOpened()
	{
		_eventService.Publish<IModMenuOpenEvent>(null);
		_wasLocked = Singleton<CursorManager>.Instance.cursorLocked;
		Singleton<CursorManager>.Instance.UnlockCursor();
		_pauseManager.Pause(state: true);
	}

	private void OnMenuClosed()
	{
		_pauseManager.Pause(state: false);
		if (_wasLocked)
		{
			CursorManager cursorManager = Singleton<CursorManager>.Instance;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			cursorManager.cursorLocked = true;
		}
	}

	public static void Run(IEnumerator routine)
	{
		if (_instance != null)
		{
			_instance.StartCoroutine(routine);
		}
	}

	public static bool IsAnyGameScene(object? sceneObj)
	{
		if (sceneObj is Scene scene)
		{
			return string.Equals(scene.name, "Game");
		}
		return false;
	}

	private static void CreateResetPoint()
	{
		_resetPoint = JsonConvert.SerializeObject(Service.Get<IModMenuService>().Modifiers, DefaultValueHelper.Settings);
		SettingsRegistryService.Registry.CaptureDefaults();
		ISettingsRegistry registry = SettingsRegistryService.Registry;
		registry.GetGroup(typeof(SideMenuGroupModMenuSettingsGeneral)).AddButton(UIText.Resource<Strings>("RESTORE_DEFAULT_VALUES"), () =>
		{
			registry.Dialogs.Confirm(UIText.Resource<Strings>("RESTORE_DEFAULT_VALUES"), UIText.Resource<Strings>("RESTORE_DEFAULT_VALUES_WARNING_TEXT"), UIText.Resource<Strings>("RESTORE_DEFAULT_VALUES"), RestoreDefaults);
		});
	}

	internal static void RestoreDefaults()
	{
		JsonConvert.PopulateObject(_resetPoint, ((ModMenuService)Service.Get<IModMenuService>())._modifierInstances, DefaultValueHelper.Settings);
		SettingsRegistryService.Registry.RefreshAll();
	}
}
