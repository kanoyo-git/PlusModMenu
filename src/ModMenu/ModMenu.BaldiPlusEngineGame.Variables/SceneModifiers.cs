using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using PlusModMenu.Core.Common;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Patches;
using ModMenu.BaldiPlusEngineGame.Settings;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Collectors;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using PlusModMenu.Api.Settings;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables;

[DataContract]
public class SceneModifiers : IModifiers
{
	private static readonly MethodInfo _destroyPlayers = AccessTools.Method(typeof(CoreGameManager), "DestroyPlayers");

	private static readonly MethodInfo _globalCamChangeType = AccessTools.Method(typeof(GlobalCam), "ChangeType");

	private bool _fullbright;

	[DataMember]
	[Label<Strings>("FULLBRIGHT")]
	[Group<SceneModifiersGeneralGroup>]
	public virtual bool Fullbright
	{
		get
		{
			return _fullbright;
		}
		set
		{
			_fullbright = value;
			if (!(Singleton<BaseGameManager>.Instance == null))
			{
				Singleton<BaseGameManager>.Instance.Ec.RestoreLighting();
			}
		}
	}

	[DataMember]
	[Label<Strings>("USE_FIXED_SEED")]
	[Help<HelpText>("USE_FIXED_SEED_TITLE", "USE_FIXED_SEED_TEXT")]
	[Group<SceneModifiersSeedGroup>]
	public virtual bool UseFixedSeed { get; set; }

	[DataMember]
	[NoDefault]
	[Label<Strings>("FIXED_SEED")]
	[Help<HelpText>("FIXED_SEED_TITLE", "FIXED_SEED_TEXT")]
	[Group<SceneModifiersSeedGroup>]
	public virtual int FixedSeed { get; set; }

	public SceneModifiers()
	{
		Service.Get<IEventService>().SubscribeOnce<IModMenuInitializedEvent>((object? _) =>
		{
			GenerateSceneManagerUI();
			GenerateSeedUI();
		});
	}

	private void GenerateSeedUI()
	{
		var group = SettingsRegistryService.Registry.GetGroup(typeof(SceneModifiersSeedGroup));
		var notInGame = UIText.Resource<Strings>("NOT_AVAILABLE_NOW");
		ISettingHandle current = null!;
		current = group.AddButton(CurrentLabel(), () =>
		{
			if (Singleton<CoreGameManager>.Instance != null)
			{
				FixedSeed = Singleton<CoreGameManager>.Instance.Seed();
				SettingsRegistryService.Registry.Refresh(this, nameof(FixedSeed));
			}
		}, UIText.Resource<HelpText>("CURRENT_SEED_TEXT"));
		var apply = group.AddButton(UIText.Resource<Strings>("APPLY_SEED_TO_NEXT_FLOORS"), () =>
		{
			Singleton<CoreGameManager>.Instance?.SetSeed(FixedSeed);
			current.Label = CurrentLabel();
		}, UIText.Resource<HelpText>("APPLY_SEED_TO_NEXT_FLOORS_TEXT"));
		current.SetAvailable(available: false, notInGame);
		apply.SetAvailable(available: false, notInGame);
		Service.Get<IEventService>().Subscribe<ISceneChangedEvent>((object? x) =>
		{
			var game = Singleton<CoreGameManager>.Instance != null;
			current.SetAvailable(game, notInGame);
			apply.SetAvailable(game, notInGame);
			current.Label = CurrentLabel();
		});
		static UIText CurrentLabel()
		{
			return Singleton<CoreGameManager>.Instance != null ? UIText.Resource<Strings>("CURRENT_SEED__SEED", Singleton<CoreGameManager>.Instance.Seed()) : UIText.Resource<Strings>("CURRENT_SEED_NONE");
		}
	}

	private static void Reset()
	{
		CoreGameManager instance = Singleton<CoreGameManager>.Instance;
		if (instance == null)
		{
			return;
		}
		BaseGameManager instance2 = Singleton<BaseGameManager>.Instance;
		if (instance2 != null)
		{
			if (instance2.Ec != null)
			{
				instance2.Ec.gameObject.SetActive(value: false);
				instance2.Ec.ResetEvents();
			}
			UnityEngine.Object.Destroy(instance2);
		}
		Singleton<GlobalCam>.Instance.SetListener(val: true);
		Singleton<SubtitleManager>.Instance.DestroyAll();
		_globalCamChangeType.Invoke(Singleton<GlobalCam>.Instance, new object[1] { Enum.ToObject(_globalCamChangeType.GetParameters()[0].ParameterType, 0) });
		Time.timeScale = 1f;
		AudioListener.pause = false;
		PropagatedAudioManager.paused = true;
		Singleton<MusicManager>.Instance.StopFile();
		Singleton<MusicManager>.Instance.SetCorruption(val: false);
		_destroyPlayers.Invoke(instance, Array.Empty<object>());
		instance.ResetShaders();
		UnityEngine.Object.DestroyImmediate(instance.gameObject);
	}

	private static void GenerateSceneManagerUI()
	{
		Dictionary<SceneObject, ISettingHandle> buttons = new Dictionary<SceneObject, ISettingHandle>();
		ISettingsRegistry registry = SettingsRegistryService.Registry;
		ISettingsGroup mainGroup = registry.GetGroup(typeof(SceneModifiersSceneManagerMainGroup));
		ISettingsGroup endlessGroup = registry.GetGroup(typeof(SceneModifiersSceneManagerEndlessGroup));
		ISettingsGroup challengeGroup = registry.GetGroup(typeof(SceneModifiersSceneManagerChallengeGroup));
		ISettingsGroup specialGroup = registry.GetGroup(typeof(SceneModifiersSceneManagerSpecialGroup));
		ISettingsGroup otherGroup = registry.GetGroup(typeof(SceneModifiersSceneManagerOtherGroup));
		SingletonCollector<SceneObject>.ItemAdded += (SceneObject x) =>
		{
			if (buttons.ContainsKey(x))
			{
				return;
			}
			ISettingHandle button;
			BaseGameManager manager = x.manager;
			if (manager is MainGameManager)
			{
				button = mainGroup.AddButton(UIText.Literal(x.levelTitle + " (" + x.name + ")"), Load, x.levelNo);
			}
			else if (manager is EndlessGameManager)
			{
				button = endlessGroup.AddButton(UIText.Literal(x.name), Load);
			}
			else if (manager is BaseChallengeGameManager)
			{
				button = challengeGroup.AddButton(UIText.Literal(x.name), Load);
			}
			else if (manager is TutorialGameManager || manager is PitstopGameManager)
			{
				button = specialGroup.AddButton(UIText.Literal(x.name), Load, (manager is TutorialGameManager) ? 0 : 1);
			}
			else
			{
				button = otherGroup.AddButton(UIText.Literal(x.name), Load);
			}
			buttons.Add(x, button);
			void Load()
			{
				LoadLevel(x);
			}
		};
		SingletonCollector<SceneObject>.ItemRemoved += (SceneObject x) =>
		{
			if (buttons.TryGetValue(x, out ISettingHandle value))
			{
				value.Remove();
				buttons.Remove(x);
			}
		};
		void LoadLevel(SceneObject sceneObject)
		{
			CoreGameManager? coreGameManagerPrefab = GameLoaderPatch.CoreGameManagerPrefab;
			if (coreGameManagerPrefab == null)
			{
				Debugger.Warning("Scene manager: the CoreGameManager prefab wasn't found, can't load " + sceneObject.name + ".");
				return;
			}
			if (Singleton<AdditiveSceneManager>.Instance == null || Singleton<AdditiveSceneManager>.Instance.Busy)
			{
				return;
			}
			Reset();
			UnityEngine.Object.Instantiate(coreGameManagerPrefab);
			CoreGameManager instance = Singleton<CoreGameManager>.Instance;
			var seed = Service.Get<IModMenuService>().Get<SceneModifiers>();
			if (seed.UseFixedSeed)
			{
				instance.SetSeed(seed.FixedSeed);
			}
			else
			{
				instance.SetRandomSeed();
			}
			instance.currentMode = Mode.Main;
			instance.SetLives((sceneObject.manager is EndlessGameManager) ? 0 : 2, lifeModeOverride: true);
			instance.lifeMode = LifeMode.Normal;
			instance.timeLimitChallenge = false;
			instance.mapChallenge = false;
			instance.inventoryChallenge = false;
			Singleton<CursorManager>.Instance.LockCursor();
			Singleton<MusicManager>.Instance.StopMidi();
			instance.sceneObject = sceneObject;
			instance.lastLevelNumber = sceneObject.levelNo;
			if (sceneObject.manager is PitstopGameManager)
			{
				instance.nextLevel = buttons.Keys.Where((SceneObject y) => y.manager is MainGameManager).OrderBy((SceneObject y) => y.levelNo).FirstOrDefault();
				if (instance.nextLevel == null)
				{
					return;
				}
			}
			Singleton<GlobalCam>.Instance.Transition(UiTransition.Dither, 0.01666667f);
			Singleton<AdditiveSceneManager>.Instance.LoadScene("Game");
			ModMenuManager.Run(BeginPlayWithoutElevator(instance));
		}
		static IEnumerator BeginPlayWithoutElevator(CoreGameManager core)
		{
			float waited = 0f;
			while (core != null && !core.readyToStart && waited < 120f)
			{
				waited += Time.unscaledDeltaTime;
				yield return null;
			}
			yield return null;
			BaseGameManager manager = Singleton<BaseGameManager>.Instance;
			if (core != null && core == Singleton<CoreGameManager>.Instance && core.readyToStart && core.disablePause && manager != null && Singleton<ElevatorScreen>.Instance == null)
			{
				manager.BeginPlay();
			}
		}
	}
}
