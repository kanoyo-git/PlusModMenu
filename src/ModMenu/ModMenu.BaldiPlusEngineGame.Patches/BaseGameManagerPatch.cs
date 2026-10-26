using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class BaseGameManagerPatch
{
	private const float DefaultAppraisalTime = 5f;

	[HarmonyPatch(typeof(BaseGameManager), "PleaseBaldi")]
	private class PleaseBaldi
	{
		private static void Prefix(ref float time)
		{
			float appraisalTime = Service.Get<IModMenuService>().Get<BaldiModifiers>().AppraisalTime;
			if (!Mathf.Approximately(appraisalTime, DefaultAppraisalTime))
			{
				time = appraisalTime;
			}
		}
	}

	[HarmonyPatch(typeof(ElevatorManager), "PlayerExitedSpawn")]
	private class PlayerExitedSpawn
	{
		private static void Prefix(bool ___playerHasExitedSpawn, out bool __state)
		{
			__state = ___playerHasExitedSpawn;
		}

		private static void Postfix(ElevatorManager __instance, bool __state)
		{
			if (!__state && !(Singleton<BaseGameManager>.Instance == null) && Singleton<BaseGameManager>.Instance.AllNotebooksFound)
			{
				__instance.SetAllElevators(ElevatorState.OpenForExit);
			}
		}
	}
}
