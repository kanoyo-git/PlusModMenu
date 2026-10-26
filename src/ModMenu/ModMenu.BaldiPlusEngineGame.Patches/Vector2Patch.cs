using System;
using PlusModMenu.Core.Common.Extensions;
using HarmonyLib;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class Vector2Patch
{
	[HarmonyPatch(typeof(Vector2), MethodType.Constructor, new Type[]
	{
		typeof(float),
		typeof(float)
	})]
	private class Constructor
	{
		private static readonly System.Random _random = new System.Random();

		private static void Prefix(ref float x, ref float y)
		{
			if (_corruptionActive && !_isSafe)
			{
				x += _random.Range(0f - _strength, _strength);
				y += _random.Range(0f - _strength, _strength);
			}
		}
	}

	private static bool _corruptionActive;

	private static bool _isSafe;

	private static float _strength = 0.1f;

	public static bool CorruptionActive
	{
		get
		{
			return _corruptionActive;
		}
		set
		{
			_corruptionActive = value;
		}
	}

	public static bool IsSafe
	{
		get
		{
			return _isSafe;
		}
		set
		{
			_isSafe = value;
		}
	}

	public static float Strength
	{
		get
		{
			return _strength;
		}
		set
		{
			_strength = value;
		}
	}
}
