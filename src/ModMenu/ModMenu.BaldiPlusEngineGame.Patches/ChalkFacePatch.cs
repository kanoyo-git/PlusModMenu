using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class ChalkFacePatch
{
	[HarmonyPatch(typeof(ChalkFace), "AdvanceTimer")]
	private class AdvanceTimer
	{
		private static void Prefix(ref float ___setTime)
		{
			___setTime = Service.Get<IModMenuService>().Get<ChalklesModifiers>().ActivationTime;
		}
	}

	[HarmonyPatch(typeof(ChalkFace), "DecreaseTimer")]
	private class DecreaseTimer
	{
		private static void Prefix(ref float ___setTime)
		{
			___setTime = Service.Get<IModMenuService>().Get<ChalklesModifiers>().ActivationTime;
		}
	}

	[HarmonyPatch(typeof(ChalkFace), "Activate")]
	private class Activate
	{
		private static void Prefix(ref float ___lockTime)
		{
			___lockTime = Service.Get<IModMenuService>().Get<ChalklesModifiers>().LockTime;
		}
	}

	[HarmonyPatch(typeof(ChalkFace), "AdvanceLaughter")]
	private class AdvanceLaughter
	{
		private static void Prefix(ref float ___spinSpeed, out float __state)
		{
			__state = ___spinSpeed;
			___spinSpeed *= Service.Get<IModMenuService>().Get<ChalklesModifiers>().Speed;
		}

		private static void Postfix(ref float ___spinSpeed, float __state)
		{
			___spinSpeed = __state;
		}
	}
}
