using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables.OtherModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Collectors;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class AudioSourcePatch
{
	[HarmonyPatch(typeof(AudioSource))]
	private class Play
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			_();
			yield return AccessTools.Method(typeof(AudioSource), "Play", Type.EmptyTypes);
			yield return AccessTools.Method(typeof(AudioSource), "Play", new Type[1] { typeof(ulong) });
			yield return AccessTools.Method(typeof(AudioSource), "PlayDelayed");
			yield return AccessTools.Method(typeof(AudioSource), "PlayScheduled");
		}

		private static void Prefix(AudioSource __instance, ref AudioClip? __state)
		{
			if (_initialized && Service.Get<IModMenuService>().Get<TrippyModifiers>().AudioVomitEnabled)
			{
				SoundObject soundObject = SingletonCollector<SoundObject>.Collection[UnityEngine.Random.Range(0, SingletonCollector<SoundObject>.Collection.Count)];
				__state = __instance.clip;
				__instance.clip = soundObject.soundClip;
			}
		}

		private static void Postfix(AudioSource __instance, AudioClip? __state)
		{
			if (__state != null)
			{
				__instance.clip = __state;
			}
		}
	}

	[HarmonyPatch(typeof(AudioSource), "PlayOneShot", new Type[]
	{
		typeof(AudioClip),
		typeof(float)
	})]
	private class PlayOneShot
	{
		private static void Prefix(ref AudioClip clip)
		{
			if (_initialized && Service.Get<IModMenuService>().Get<TrippyModifiers>().AudioVomitEnabled)
			{
				SoundObject soundObject = SingletonCollector<SoundObject>.Collection[UnityEngine.Random.Range(0, SingletonCollector<SoundObject>.Collection.Count)];
				clip = soundObject.soundClip;
			}
		}
	}

	private static bool _initialized;

	static AudioSourcePatch()
	{
		Service.Get<IEventService>().SubscribeOnce<IModMenuInitializedEvent>((object? _) =>
		{
			_initialized = true;
		});
	}

	private static void _()
	{
	}
}
