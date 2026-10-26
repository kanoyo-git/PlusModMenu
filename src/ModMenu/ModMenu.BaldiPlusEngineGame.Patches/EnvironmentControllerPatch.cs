using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Interfaces;
using ModMenu.BaldiPlusEngineGame.Variables;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class EnvironmentControllerPatch
{
	[HarmonyPatch(typeof(EnvironmentController), "UpdateFog")]
	private class UpdateFog
	{
		private static readonly int _fogStrength = Shader.PropertyToID("_FogStrength");

		private static readonly int _fogActive = Shader.PropertyToID("_FogActive");

		private static bool Prefix()
		{
			if (!Service.Get<IModMenuService>().Get<SceneModifiers>().Fullbright)
			{
				return true;
			}
			Shader.SetGlobalFloat(_fogStrength, 0f);
			Shader.SetGlobalInt(_fogActive, 0);
			return false;
		}
	}

	[HarmonyPatch(typeof(EnvironmentController), "SpawnNPC")]
	private class SpawnNPC
	{
		private static void Postfix(NPC __result)
		{
			if (!(__result == null) && Service.Get<ICharacterHelperService>().GetSpecificCharacterModifiersOfNPC(typeof(ICharacterLanternModeModifiers), __result.GetType()) is ICharacterLanternModeModifiers { EmitsLightDuringLightsOut: not false } characterLanternModeModifiers)
			{
				LanternModePatch.LanternModeHelper.Instance?.LanternMode?.AddSource(__result.transform, characterLanternModeModifiers.LightsOutStrength, characterLanternModeModifiers.LightsOutLightColor);
			}
		}
	}
}
