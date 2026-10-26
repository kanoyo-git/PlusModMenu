using System.Reflection;
using PlusModMenu.Core.Common.Extensions;
using HarmonyLib;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal static class EnvironmentControllerLighting
{
	private static readonly FieldInfo _lightMap = AccessTools.Field(typeof(EnvironmentController), "lightMap");

	public static void RestoreLighting(this EnvironmentController? ec)
	{
		if (ec == null || ec.lightingOverride || Singleton<CoreGameManager>.Instance == null)
		{
			return;
		}
		LightingController[,]? lightMap = _lightMap.GetValue<LightingController[,]>(ec);
		if (lightMap == null)
		{
			return;
		}
		foreach (LightingController lightingController in lightMap)
		{
			if (lightingController != null)
			{
				lightingController.UpdateLighting();
				Singleton<CoreGameManager>.Instance.UpdateLighting(lightingController.Color, lightingController.position);
			}
		}
	}
}
