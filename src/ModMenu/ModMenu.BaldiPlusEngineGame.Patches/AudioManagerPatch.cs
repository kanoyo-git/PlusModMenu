using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Patches.Helpers;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class AudioManagerPatch
{
	[HarmonyPatch(typeof(AudioManager))]
	private class AllMethods : ReplaceCoroutineCalls<AudioManager>
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			return ReplaceCoroutineCalls<AudioManager>._TargetMethods();
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return ReplaceCoroutineCalls<AudioManager>._Transpiler(instructions);
		}
	}

	[HarmonyPatch(typeof(AudioManager), "Awake")]
	private class Awake
	{
		private static void Postfix(AudioManager __instance)
		{
			__instance.gameObject.AddComponent<AudioManagerHelper>();
		}
	}

	private class AudioManagerHelper : MonoBehaviour
	{
		private AudioManager _audioManager;

		private bool _wasPlaying;

		private void Awake()
		{
			_audioManager = GetComponent<AudioManager>();
		}

		private void OnEnable()
		{
			if (_wasPlaying)
			{
				_audioManager.Pause(pause: false);
			}
		}

		private void OnDisable()
		{
			try
			{
				_wasPlaying = _audioManager != null && _audioManager.QueuedAudioIsPlaying;
				if (_wasPlaying)
				{
					_audioManager.Pause(pause: true);
				}
			}
			catch (NullReferenceException)
			{
				_wasPlaying = false;
			}
		}
	}
}
