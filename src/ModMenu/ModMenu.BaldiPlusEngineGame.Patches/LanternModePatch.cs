using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PlusModMenu.Core.Common.Extensions;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Interfaces;
using ModMenu.BaldiPlusEngineGame.Variables;
using PlusModMenu.Api;
using PlusModMenu.Api.Characters;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class LanternModePatch
{
	[HarmonyPatch(typeof(LanternMode), "Initialize")]
	private class Initialize
	{
		private static bool Prefix(LanternMode __instance)
		{
			LanternModeHelper? instance = LanternModeHelper.Instance;
			if (instance != null)
			{
				return instance.LanternMode == __instance;
			}
			LanternModeHelper.Reset();
			LanternModeHelper lanternModeHelper = __instance.gameObject.AddComponent<LanternModeHelper>();
			lanternModeHelper.NormalSkybox = Shader.GetGlobalTexture("_Skybox") as Cubemap;
			lanternModeHelper.LanternMode = __instance;
			return true;
		}
	}

	[HarmonyPatch(typeof(LanternMode), "AddSource")]
	private class AddSource
	{
		private static bool Prefix(Transform trans, List<LanternSource> ___sources)
		{
			LanternSource lanternSource = ___sources.FirstOrDefault((LanternSource x) => x.transform == trans);
			if (lanternSource == null)
			{
				return true;
			}
			NPC component = trans.GetComponent<NPC>();
			if ((object)component != null)
			{
				if (Service.Get<ICharacterHelperService>().GetSpecificCharacterModifiersOfNPC(typeof(ICharacterLanternModeModifiers), component.GetType()) is ICharacterLanternModeModifiers characterLanternModeModifiers)
				{
					lanternSource.strength = characterLanternModeModifiers.LightsOutStrength;
					lanternSource.color = characterLanternModeModifiers.LightsOutLightColor;
				}
			}
			else if ((bool)trans.GetComponent<PlayerMovement>())
			{
				PlayerModifiers playerModifiers = Service.Get<IModMenuService>().Get<PlayerModifiers>();
				lanternSource.strength = playerModifiers.LightsOutStrength;
				lanternSource.color = playerModifiers.LightsOutLightColor;
			}
			return false;
		}
	}

	[HarmonyPatch(typeof(LanternMode), "Update")]
	private class Update
	{
		private static void Prefix(List<LanternSource> ___sources)
		{
			___sources.RemoveAll((LanternSource x) => x.transform == null);
		}
	}

	internal class LanternModeHelper : MonoBehaviour
	{
		private static readonly FieldInfo _sources = AccessTools.Field(typeof(LanternMode), "sources");

		private static readonly FieldInfo _ec = AccessTools.Field(typeof(LanternMode), "ec");

		private static readonly Cubemap _blackSkybox = new Cubemap(1, DefaultFormat.HDR, TextureCreationFlags.None);

		public static LanternModeHelper? Instance { get; private set; }

		public LanternMode? LanternMode { get; set; }

		public Cubemap? NormalSkybox { get; set; }

		public List<LanternSource> Sources
		{
			get
			{
				if (!(LanternMode == null))
				{
					return _sources.GetValue<List<LanternSource>>(LanternMode);
				}
				return new List<LanternSource>();
			}
		}

		public bool Enabled
		{
			get
			{
				if (!(LanternMode == null))
				{
					return LanternMode.enabled;
				}
				return false;
			}
			set
			{
				Shader.SetGlobalTexture("_Skybox", value ? _blackSkybox : NormalSkybox);
				EnvironmentController? ec = (LanternMode == null) ? null : _ec.GetValue<EnvironmentController>(LanternMode);
				if (LanternMode != null)
				{
					LanternMode.enabled = value;
				}
				if (ec != null)
				{
					ec.lightingOverride = value;
					ec.RestoreLighting();
				}
			}
		}

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
		}

		private void Start()
		{
			if (Instance != this)
			{
				Object.Destroy(this);
				return;
			}
			UpdateSources();
		}

		private void OnDestroy()
		{
			if (Instance == this)
			{
				Instance = null;
			}
		}

		public void UpdateSources()
		{
			IModMenuService modMenuService = Service.Get<IModMenuService>();
			modMenuService.Get<PlayerModifiers>().EmitsLightDuringLightsOut = modMenuService.Get<PlayerModifiers>().EmitsLightDuringLightsOut;
			foreach (ILanternModeSourceModifiers item in modMenuService.Modifiers.OfType<ILanternModeSourceModifiers>())
			{
				item.EmitsLightDuringLightsOut = item.EmitsLightDuringLightsOut;
			}
		}

		public static void Reset()
		{
			Instance = null;
		}

		public static LanternMode CreateLanternMode(GameObject gameObject)
		{
			return gameObject.AddComponent<LanternMode>();
		}

		public static void SetEmitsLightDuringLightsOut(IReadOnlyCollection<CharacterCollectorItem> collection, bool value, float strength, Color color)
		{
			LanternModeHelper instance = Instance;
			if ((object)instance == null)
			{
				return;
			}
			LanternMode lanternMode = instance.LanternMode;
			if ((object)lanternMode == null)
			{
				return;
			}
			if (value)
			{
				foreach (CharacterCollectorItem item in collection)
				{
					lanternMode.AddSource(item.transform, strength, color);
				}
				return;
			}
			foreach (CharacterCollectorItem characterCollectorItem in collection)
			{
				instance.Sources.RemoveAll((LanternSource x) => x.transform == characterCollectorItem.transform);
			}
		}

		public static void SetLightColor(IReadOnlyCollection<CharacterCollectorItem> collection, Color value)
		{
			LanternModeHelper instance = Instance;
			if ((object)instance == null || (object)instance.LanternMode == null)
			{
				return;
			}
			foreach (LanternSource item in instance.Sources.Where((LanternSource x) => collection.Any((CharacterCollectorItem y) => y.transform == x.transform)))
			{
				item.color = value;
			}
		}

		public static void SetLightStrength(IReadOnlyCollection<CharacterCollectorItem> collection, float value)
		{
			LanternModeHelper instance = Instance;
			if ((object)instance == null || (object)instance.LanternMode == null)
			{
				return;
			}
			foreach (LanternSource item in instance.Sources.Where((LanternSource x) => collection.Any((CharacterCollectorItem y) => y.transform == x.transform)))
			{
				item.strength = value;
			}
		}
	}
}
