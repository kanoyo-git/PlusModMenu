using System;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Clipboard;
using ModMenu.BaldiPlusEngineGame.Variables;
using ModMenu.Common.CustomComponents;
using PlusModMenu.Api;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class CoreGameManagerPatch
{
	[HarmonyPatch(typeof(CoreGameManager), "SpawnPlayers")]
	private class SpawnPlayers
	{
		private static void Postfix(CoreGameManager __instance, EnvironmentController ec)
		{
			GPSText.CalcDistanceFrom = __instance.GetPlayer(0).transform;
			EndGameInProgress = false;
			PlayersSpawned?.Invoke(ec);
			Service.Get<IEventService>().Publish<ILevelStartedEvent>(ec);
		}
	}

	[HarmonyPatch(typeof(CoreGameManager), "ReturnToMenu")]
	private class ReturnToMenu
	{
		private static void Postfix()
		{
			EndGameInProgress = false;
		}
	}

	[HarmonyPatch(typeof(CoreGameManager), "UpdateLighting")]
	private class UpdateLighting
	{
		private static void Prefix(ref Color color)
		{
			if (Service.Get<IModMenuService>().Get<SceneModifiers>().Fullbright)
			{
				color = Color.white;
			}
		}
	}

	[HarmonyPatch(typeof(CoreGameManager), "EndGame")]
	private class EndGame
	{
		private static bool Prefix()
		{
			if (EndGameInProgress)
			{
				return false;
			}
			return EndGameInProgress = true;
		}
	}

	[HarmonyPatch(typeof(CoreGameManager), "Pause")]
	private class Pause
	{
		private static bool Prefix()
		{
			return !ClipboardMenu.BlocksGame;
		}
	}

	internal static bool EndGameInProgress { get; private set; }

	internal static event Action<EnvironmentController>? PlayersSpawned;
}
