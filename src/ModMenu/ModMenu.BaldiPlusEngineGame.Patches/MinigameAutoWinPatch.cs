using System.Collections;
using System.Reflection;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables.OtherModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

[HarmonyPatch(typeof(MinigameBase), nameof(MinigameBase.TitleSequenceFinished))]
internal static class MinigameAutoWinPatch
{
	private static readonly FieldInfo _scoringMode = AccessTools.Field(typeof(MinigameBase), "scoringMode");

	internal static int Won { get; private set; }

	private static void Postfix(MinigameBase __instance)
	{
		if (Service.Get<IModMenuService>().Get<WorldModifiers>().MinigameAutoWin && !(bool)_scoringMode.GetValue(__instance))
		{
			__instance.StartCoroutine(Win(__instance));
		}
	}

	private static IEnumerator Win(MinigameBase minigame)
	{
		yield return null;
		if (minigame == null)
		{
			yield break;
		}
		minigame.AwardPerfectBonus();
		minigame.Win();
		Won++;
	}
}
