using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using PlusModMenu.Api.Characters;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Utils;

public class CharacterUtils
{
	private static readonly FieldInfo _disableSubtitles = AccessTools.Field(typeof(AudioManager), "disableSubtitles");

	private static readonly SoundType[] _soundTypes = (SoundType[])Enum.GetValues(typeof(SoundType));

	public static void SetSilence(out bool state, bool value, IReadOnlyList<CharacterCollectorItem> collection)
	{
		state = value;
		SubtitleController[] subs = Resources.FindObjectsOfTypeAll<SubtitleController>();
		foreach (CharacterCollectorItem item in collection)
		{
			InternalSetSilence(subs, item, value);
		}
	}

	public static void SetSilence(out bool state, bool value, Component c)
	{
		state = value;
		InternalSetSilence(Resources.FindObjectsOfTypeAll<SubtitleController>(), c, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void InternalSetSilence(SubtitleController[] subs, Component c, bool value)
	{
		foreach (SubtitleController item in subs.Where((SubtitleController x) => x.soundTran == c.transform))
		{
			item.gameObject.SetActive(!value);
		}
		AudioManager[] componentsInChildren = c.GetComponentsInChildren<AudioManager>();
		foreach (AudioManager audioManager in componentsInChildren)
		{
			if (audioManager.audioSourceManager != null)
			{
				foreach (SoundType soundType in _soundTypes)
				{
					AudioSource audioSource = audioManager.audioSourceManager.GetAudioSource(soundType);
					if (audioSource != null)
					{
						audioSource.mute = value;
					}
				}
			}
			else if (audioManager.audioDevice != null)
			{
				audioManager.audioDevice.mute = value;
			}
			_disableSubtitles.SetValue(audioManager, value);
		}
	}

	public static void SetScale(out float state, float value, IReadOnlyList<CharacterCollectorItem> collection)
	{
		state = value;
		foreach (CharacterCollectorItem item in collection)
		{
			InternalSetScale(item, value);
		}
	}

	public static void SetScale(out float state, float value, Component c)
	{
		state = value;
		InternalSetScale(c, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void InternalSetScale(Component c, float value)
	{
		c.transform.localScale = Vector3.one * value;
	}

	public static void SetInvisible(out bool state, bool value, IReadOnlyList<CharacterCollectorItem> collection)
	{
		state = value;
		uint renderingLayerMask = ((!value) ? 1u : 0u);
		foreach (CharacterCollectorItem item in collection)
		{
			InternalSetInvisible(renderingLayerMask, item);
		}
	}

	public static void SetInvisible(out bool state, bool value, Component c)
	{
		state = value;
		InternalSetInvisible((!value) ? 1u : 0u, c);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void InternalSetInvisible(uint renderingLayerMask, Component c)
	{
		SpriteRenderer[] componentsInChildren = c.GetComponentsInChildren<SpriteRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].renderingLayerMask = renderingLayerMask;
		}
	}
}
