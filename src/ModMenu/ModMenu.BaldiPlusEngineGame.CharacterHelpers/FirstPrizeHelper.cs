using System;
using System.Reflection;
using PlusModMenu.Core.Common.Extensions;
using HarmonyLib;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.CharacterHelpers;

internal class FirstPrizeHelper : MonoBehaviour
{
	private static readonly FieldInfo _currentPlayer = AccessTools.Field(typeof(FirstPrize_Active), "currentPlayer");

	private static readonly MethodInfo _playerExitedTrigger = AccessTools.Method(typeof(FirstPrize_Active), "PlayerExitedTrigger");

	private FirstPrize _firstPrize;

	private void Awake()
	{
		_firstPrize = GetComponent<FirstPrize>();
		if (_firstPrize.motorAudMan != null && _firstPrize.motorAudMan.audioSourceManager != null)
		{
			foreach (SoundType soundType in (SoundType[])Enum.GetValues(typeof(SoundType)))
			{
				AudioSource audioSource = _firstPrize.motorAudMan.audioSourceManager.GetAudioSource(soundType);
				if (audioSource != null)
				{
					audioSource.playOnAwake = true;
				}
			}
		}
	}

	private void OnDisable()
	{
		if (_firstPrize != null && _firstPrize.behaviorStateMachine.CurrentState is FirstPrize_Active firstPrizeActive)
		{
			PlayerManager value = _currentPlayer.GetValue<PlayerManager>(firstPrizeActive);
			if (value != null)
			{
				_playerExitedTrigger.Invoke(firstPrizeActive, new object[1] { value.transform });
			}
		}
	}
}
