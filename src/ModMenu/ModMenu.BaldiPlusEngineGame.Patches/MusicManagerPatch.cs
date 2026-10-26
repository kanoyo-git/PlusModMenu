using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using PlusModMenu.Core.Harmony.Extensions;
using HarmonyLib;
using MidiPlayerTK;
using ModMenu.BaldiPlusEngineGame.Variables.OtherModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class MusicManagerPatch
{
	[HarmonyPatch]
	private class PreventOnSongLock
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(typeof(MusicManager), "PlayMidi", new Type[2]
			{
				typeof(string),
				typeof(bool)
			});
			yield return AccessTools.Method(typeof(MusicManager), "PlayMidi", new Type[3]
			{
				typeof(string),
				typeof(float),
				typeof(bool)
			});
			yield return AccessTools.Method(typeof(MusicManager), "QueueFile", new Type[2]
			{
				typeof(LoopingSoundObject),
				typeof(bool)
			});
			yield return AccessTools.Method(typeof(MusicManager), "StartFileQueue");
			yield return AccessTools.Method(typeof(MusicManager), "StopFile");
			yield return AccessTools.Method(typeof(MusicManager), "PauseMidi", new Type[1] { typeof(bool) });
			yield return AccessTools.Method(typeof(MusicManager), "StopMidi");
			yield return AccessTools.Method(typeof(MusicManager), "SetLoop", new Type[1] { typeof(bool) });
			yield return AccessTools.Method(typeof(MusicManager), "SetSpeed", new Type[1] { typeof(float) });
			yield return AccessTools.Method(typeof(MusicManager), "ModulateSpeed", new Type[5]
			{
				typeof(float),
				typeof(float),
				typeof(float),
				typeof(float),
				typeof(bool)
			});
			yield return AccessTools.Method(typeof(MusicManager), "StopModulation");
			yield return AccessTools.Method(typeof(MusicManager), "StartExponentialModulator");
			yield return AccessTools.Method(typeof(MusicManager), "MidiLoop");
			yield return AccessTools.Method(typeof(MusicManager), "TransitionFinished");
			yield return AccessTools.Method(typeof(MusicManager), "SetCorruption", new Type[1] { typeof(bool) });
			yield return AccessTools.Method(typeof(MusicManager), "LoadCorruptionData", new Type[1] { typeof(MidiCorruptionObject) });
		}

		private static bool Prefix()
		{
			AudioModifiers audioModifiers = Service.Get<IModMenuService>().Get<AudioModifiers>();
			if (audioModifiers != null && audioModifiers.LockSongSelection)
			{
				return audioModifiers.BypassLock;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(MusicManager), "PlayMidi", new Type[]
	{
		typeof(string),
		typeof(float),
		typeof(bool)
	})]
	private class PlayMidi
	{
		private static bool LockPitch => Service.Get<IModMenuService>().Get<AudioModifiers>().LockPitch;

		private static void Prefix(MidiFilePlayer ___midiPlayer, out int __state)
		{
			__state = ___midiPlayer.transpose;
		}

		private static void Postfix(MidiFilePlayer ___midiPlayer, int __state)
		{
			if (LockPitch)
			{
				___midiPlayer.transpose = __state;
			}
		}
	}

	[HarmonyPatch(typeof(MusicManager), "SetCorruption")]
	private class SetCorruption
	{
		private static bool Prefix(out bool __state)
		{
			AudioModifiers audioModifiers = Service.Get<IModMenuService>().Get<AudioModifiers>();
			return __state = !audioModifiers.LockMusicCorruption || audioModifiers.BypassLock;
		}

		private static void Postfix(bool val, bool __state)
		{
			if (__state)
			{
				Service.Get<IModMenuService>().Get<AudioModifiers>().UpdateControl("MusicCorruption", val);
			}
		}
	}
}
