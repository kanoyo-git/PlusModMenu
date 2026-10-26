using System.Linq;
using PlusModMenu.Core.Common.Utils;
using UnityEngine;

namespace PlusModMenu.Api.Internal;

internal class Internals
{
	private static NPC[]? _characters;

	internal static NPC[] Characters
	{
		get
		{
			if (_characters == null || _characters.Length < 1)
			{
				return _characters = (from x in Resources.FindObjectsOfTypeAll<NPC>().Reverse()
					where !x.name.Contains("Classic") && !x.name.Contains("GLITCH")
					select x).DistinctBy((NPC x) => x.GetType()).ToArray();
			}
			return _characters;
		}
	}

	internal static float CharacterActivationDistance { get; set; }
}
