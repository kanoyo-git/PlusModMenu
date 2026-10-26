using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClassicYetAgain.Variables.Menus;
using PlusModMenu.Core.Common;
using PlusModMenu.Core.Unity;
using PlusModMenu.Core.Unity.Utils;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Patches;
using ModMenu.BaldiPlusEngineGame.Settings;
using PlusModMenu.Api;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Collectors;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using PlusModMenu.Api.Settings;
using UnityEngine;

namespace ClassicYetAgain.Variables;

[DataContract]
public class OtherModifiers : IModifiers
{
	private class StormyNightManager
	{
		private static Cubemap _nightTimeSkybox;

		private readonly TaskManager.SimpleTask _stormyNightTask;

		private readonly SoundObject _rainLoopSound;

		private readonly SoundObject _thunderSound;

		private Cubemap? _originalCubemap;

		private GameObject? _audioManagerGameObject;

		private bool _running;

		private static readonly AccessTools.FieldRef<EnvironmentController, LightingController[,]> _lightMap = AccessTools.FieldRefAccess<EnvironmentController, LightingController[,]>("lightMap");

		private EnvironmentController? _ec;

		private bool _previousLightingOverride;

		public static Cubemap NightTimeSkybox
		{
			set
			{
				_nightTimeSkybox = value;
			}
		}

		public StormyNightManager()
		{
			_stormyNightTask = new TaskManager.SimpleTask(StormyNightRoutine);
			_rainLoopSound = ScriptableObject.CreateInstance<SoundObject>();
			_rainLoopSound.soundKey = "CYA_SFX_RainLoop";
			_rainLoopSound.soundClip = PlusModMenu.Core.Unity.ResourceManager.Get("RainLoop.ogg");
			_rainLoopSound.soundType = SoundType.Effect;
			_thunderSound = ScriptableObject.CreateInstance<SoundObject>();
			_thunderSound.soundKey = "CYA_SFX_Thunder";
			_thunderSound.soundClip = PlusModMenu.Core.Unity.ResourceManager.Get("Thunder.ogg");
			_thunderSound.soundType = SoundType.Effect;
			Service.Get<IEventService>().Subscribe<ISceneChangedEvent>((object? _) =>
			{
				Stop();
			});
		}

		public void Start()
		{
			if (!_running)
			{
				_stormyNightTask.Reset();
				_stormyNightTask.Start();
				_running = true;
			}
		}

		public void Stop()
		{
			if (_running)
			{
				_stormyNightTask.Stop();
				Shader.SetGlobalTexture("_Skybox", _originalCubemap);
				RestoreLighting();
				UnityEngine.Object.Destroy(_audioManagerGameObject);
				_running = false;
			}
		}

		private void RestoreLighting()
		{
			EnvironmentController? ec = _ec;
			_ec = null;
			if (ec == null || Singleton<CoreGameManager>.Instance == null)
			{
				return;
			}
			ec.lightingOverride = _previousLightingOverride || (LanternModePatch.LanternModeHelper.Instance?.Enabled ?? false);
			if (ec.lightingOverride)
			{
				return;
			}
			LightingController[,] lightMap = _lightMap(ec);
			for (int i = 0; i < lightMap.GetLength(0); i++)
			{
				for (int j = 0; j < lightMap.GetLength(1); j++)
				{
					if (lightMap[i, j] != null)
					{
						ec.QueueLightControllerForUpdate(lightMap[i, j]);
					}
				}
			}
		}

		private IEnumerator StormyNightRoutine()
		{
			while (Singleton<BaseGameManager>.Instance == null || Singleton<BaseGameManager>.Instance.Ec == null || Singleton<CoreGameManager>.Instance == null || Singleton<CoreGameManager>.Instance.GetPlayer(0) == null)
			{
				yield return null;
			}
			_ec = Singleton<BaseGameManager>.Instance.Ec;
			_previousLightingOverride = _ec.lightingOverride;
			_ec.lightingOverride = true;
			Color darkColor = new Color(0.15f, 0.15f, 0.15f);
			SetLightingColor(darkColor);
			_originalCubemap = (Cubemap)Shader.GetGlobalTexture("_Skybox");
			Shader.SetGlobalTexture("_Skybox", _nightTimeSkybox);
			StormyNightManager stormyNightManager = this;
			GameObject gameObject = new GameObject();
			gameObject.transform.parent = Singleton<CoreGameManager>.Instance.GetPlayer(0).transform;
			gameObject.transform.localPosition = Vector3.zero;
			stormyNightManager._audioManagerGameObject = gameObject;
			AudioManager audioManager = _audioManagerGameObject.AddComponent<AudioManager>();
			audioManager.audioDevice = _audioManagerGameObject.AddComponent<AudioSource>();
			audioManager.maintainLoop = true;
			audioManager.loop = true;
			audioManager.QueueAudio(_rainLoopSound);
			while (true)
			{
				float startTime = Time.time;
				float fadeTime = 0f;
				int doubleChance = UnityEngine.Random.Range(0, 100);
				yield return new WaitForSeconds(0.2f);
				audioManager.PlaySingle(_thunderSound);
				while (fadeTime < 1f)
				{
					fadeTime = (Time.time - startTime) * 0.5f;
					SetLightingColor(Color.Lerp(Color.white, darkColor, fadeTime));
					if (doubleChance >= 70 && fadeTime > 0.2f)
					{
						startTime = Time.time;
						doubleChance = 0;
					}
					yield return null;
				}
				yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 60f));
			}
			static void SetLightingColor(Color color)
			{
				IntVector2 pos = default;
				EnvironmentController ec = Singleton<BaseGameManager>.Instance.Ec;
				CoreGameManager instance = Singleton<CoreGameManager>.Instance;
				if (ec == null || instance == null)
				{
					return;
				}
				for (int i = 0; i < ec.levelSize.x; i++)
				{
					for (int j = 0; j < ec.levelSize.z; j++)
					{
						pos.x = i;
						pos.z = j;
						instance.UpdateLighting(color, pos);
					}
				}
			}
		}
	}

	private static readonly StormyNightManager _stormyNightManager = new StormyNightManager();

	private bool _stormyNight;

	[DataMember]
	[Label<Strings>("ENFORCE_SELECTED_SKYBOX")]
	[Help<HelpText>("ENFORCE_SELECTED_SKYBOX_TITLE", "ENFORCE_SELECTED_SKYBOX_TEXT")]
	[Group<SideMenuGroupOtherModifiersSkyboxGeneral>]
	public virtual bool EnforceSelectedSkybox { get; set; }

	[DataMember]
	[NoDefault]
	[Label<Strings>("SKYBOX_COLOR")]
	[Help<HelpText>("SKYBOX_COLOR_TITLE", "SKYBOX_COLOR_TEXT")]
	[Group<SideMenuGroupOtherModifiersSkyboxGeneral>]
	public virtual Color SkyboxColor
	{
		get
		{
			return Shader.GetGlobalColor("_SkyboxColor");
		}
		set
		{
			Shader.SetGlobalColor("_SkyboxColor", value);
		}
	}

	[DataMember]
	[Label<Strings>("STORMY_NIGHT")]
	[Help<HelpText>("STORMY_NIGHT_TITLE", "STORMY_NIGHT_TEXT")]
	[Group<SideMenuGroupOtherModifiersSkyboxGeneral>]
	public virtual bool StormyNight
	{
		get
		{
			return _stormyNight;
		}
		set
		{
			_stormyNight = value;
			if (value)
			{
				_stormyNightManager.Start();
			}
			else
			{
				_stormyNightManager.Stop();
			}
		}
	}

	public OtherModifiers()
	{
		Service.Get<IEventService>().Subscribe<IModMenuOpenEvent>((object? _) =>
		{
			SkyboxColor = SkyboxColor;
		});
		Service.Get<IEventService>().Subscribe<ILevelStartedEvent>((object? _) =>
		{
			StormyNight = StormyNight;
		});
		Service.Get<IEventService>().SubscribeOnce<IModMenuInitializedEvent>((object? _) =>
		{
			GenerateSkyboxManagerUI();
		});
	}

	private static void GenerateSkyboxManagerUI()
	{
		Dictionary<Cubemap, ISettingHandle> buttons = new Dictionary<Cubemap, ISettingHandle>();
		ISettingsGroup group = SettingsRegistryService.Registry.GetPage(typeof(SideMenuMenuOtherModifiersSkybox)).AddGroup(UIText.Resource<Strings>("SKYBOX"));
		SingletonCollector<SceneObject>.ItemAdded += (SceneObject x) =>
		{
			if (x.skybox != null && !buttons.ContainsKey(x.skybox))
			{
				buttons.Add(x.skybox, group.AddButton(UIText.Literal(x.skybox.name.Replace("Cubemap_", "").Replace("Standard", "")), () =>
				{
					Shader.SetGlobalTexture("_Skybox", x.skybox);
				}));
			}
		};
		SingletonCollector<SceneObject>.ItemRemoved += (SceneObject x) =>
		{
			if (x.skybox != null && buttons.TryGetValue(x.skybox, out ISettingHandle button))
			{
				button.Remove();
				buttons.Remove(x.skybox);
			}
		};
		Texture2D texture = TextureUtils.CreateFromImageData(PlusModMenu.Core.Common.ResourceManager.GetBytes("darksky_cubemap.png"));
		Cubemap cubemap = TextureUtils.CubemapFromTexture2D(texture);
		buttons.Add(cubemap, group.AddButton(UIText.Literal("Night"), () =>
		{
			Shader.SetGlobalTexture("_Skybox", cubemap);
		}));
		StormyNightManager.NightTimeSkybox = cubemap;
	}
}
