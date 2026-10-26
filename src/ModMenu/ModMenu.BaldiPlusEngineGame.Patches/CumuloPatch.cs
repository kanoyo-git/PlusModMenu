using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.CharacterHelpers;
using ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class CumuloPatch
{
	[HarmonyPatch(typeof(Cumulo), "Initialize")]
	private class Start
	{
		private static void Postfix(Cumulo __instance)
		{
			__instance.gameObject.AddComponent<CloudyCopterHelper>();
		}
	}

	[HarmonyPatch(typeof(Cumulo), "RandomBlowTime", MethodType.Getter)]
	private class Blow
	{
		private static void Prefix(ref float ___minStay, ref float ___maxStay)
		{
			CloudyCopterModifiers cloudyCopterModifiers = Service.Get<IModMenuService>().Get<CloudyCopterModifiers>();
			___minStay = cloudyCopterModifiers.MinStay;
			___maxStay = cloudyCopterModifiers.MaxStay;
		}
	}

	[HarmonyPatch(typeof(Cumulo), "FindDestination")]
	private class FindDestination
	{
		private static void Prefix(ref int ___minHallLength)
		{
			CloudyCopterModifiers cloudyCopterModifiers = Service.Get<IModMenuService>().Get<CloudyCopterModifiers>();
			___minHallLength = cloudyCopterModifiers.MinHallLength;
		}
	}
}
