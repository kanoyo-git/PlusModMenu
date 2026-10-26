using System.Reflection;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class GravityEventPatch
{
	[HarmonyPatch(typeof(RandomEvent), "Initialize")]
	private class Initialize
	{
		private static readonly FieldInfo _minFlippersField = AccessTools.Field(typeof(GravityEvent), "minFlippers");

		private static readonly FieldInfo _maxFlippersField = AccessTools.Field(typeof(GravityEvent), "maxFlippers");

		private static readonly FieldInfo _initialSpawnDelayField = AccessTools.Field(typeof(GravityEvent), "initialSpawnDelay");

		private static readonly FieldInfo _minRespawnTimeField = AccessTools.Field(typeof(GravityEvent), "minRespawnTime");

		private static readonly FieldInfo _maxRespawnTimeField = AccessTools.Field(typeof(GravityEvent), "maxRespawnTime");

		private static void Prefix(RandomEvent __instance)
		{
			if (__instance is GravityEvent)
			{
				GravityEventModifiers gravityEventModifiers = Service.Get<IModMenuService>().Get<GravityEventModifiers>();
				_minFlippersField.SetValue(__instance, gravityEventModifiers.MinFlippers);
				_maxFlippersField.SetValue(__instance, gravityEventModifiers.MaxFlippers);
				_initialSpawnDelayField.SetValue(__instance, gravityEventModifiers.InitialSpawnDelay);
				_minRespawnTimeField.SetValue(__instance, gravityEventModifiers.MinRespawnTime);
				_maxRespawnTimeField.SetValue(__instance, gravityEventModifiers.MaxRespawnTime);
			}
		}
	}
}
