using System;
using PlusModMenu.Core.Common.Extensions;
using HarmonyLib;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class Vector3Patch
{
	[HarmonyPatch(typeof(Vector3), MethodType.Constructor, new Type[]
	{
		typeof(float),
		typeof(float),
		typeof(float)
	})]
	private class Constructor
	{
		private static readonly System.Random _random = new System.Random();

		private static void Prefix(ref float x, ref float y, ref float z)
		{
			if (_corruptionActive && !_isSafe)
			{
				x += _random.Range(0f - _strength, _strength);
				y += _random.Range(0f - _strength, _strength);
				z += _random.Range(0f - _strength, _strength);
			}
		}
	}

	private static bool _corruptionActive;

	private static bool _isSafe;

	private static float _strength = 1f;

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
