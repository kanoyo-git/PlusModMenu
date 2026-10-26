using System.Collections;
using System.Reflection;
using PlusModMenu.Core.Common.Extensions;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class FogEventPatch
{
	private static readonly FieldInfo _ecField = AccessTools.Field(typeof(RandomEvent), "ec");

	[HarmonyPatch(typeof(FogEvent), "FadeOffFog")]
	private class FadeOffFog
	{
		private static void Postfix(FogEvent __instance, ref IEnumerator __result)
		{
			__result = RemoveFogAfter(__instance, __result);
		}

		private static IEnumerator RemoveFogAfter(FogEvent fogEvent, IEnumerator original)
		{
			while (original.MoveNext())
			{
				yield return original.Current;
			}
			_ecField.GetValue<EnvironmentController>(fogEvent).RemoveFog(fogEvent.fog);
		}
	}

	[HarmonyPatch(typeof(FogEvent), "FadeOnFog")]
	private class FadeOnFog
	{
		private static float FogStrength => Service.Get<IModMenuService>().Get<FogEventModifiers>().FogStrength;

		private static bool Prefix(FogEvent __instance, ref IEnumerator __result)
		{
			__result = FadeOnFogModified(__instance, _ecField.GetValue<EnvironmentController>(__instance));
			return false;
		}

		private static IEnumerator FadeOnFogModified(FogEvent fogEvent, EnvironmentController ec)
		{
			Fog fog = fogEvent.fog;
			ec.AddFog(fog);
			SetFogValues(fog);
			fog.strength = 0f;
			float fogStrength = 0f;
			while (fogStrength < FogStrength)
			{
				fogStrength += 0.25f * Time.deltaTime;
				fog.strength = fogStrength;
				ec.UpdateFog();
				yield return null;
				SetFogValues(fog);
			}
			fogStrength = FogStrength;
			fog.strength = fogStrength;
			ec.UpdateFog();
		}

		private static void SetFogValues(Fog fog)
		{
			FogEventModifiers fogEventModifiers = Service.Get<IModMenuService>().Get<FogEventModifiers>();
			fog.color = fogEventModifiers.FogColor;
			fog.startDist = fogEventModifiers.StartDist;
			fog.maxDist = fogEventModifiers.MaxDist;
		}
	}
}
