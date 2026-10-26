using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables.OtherModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using TMPro;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class TMP_TextPatch
{
	[HarmonyPatch(typeof(TMP_Text), "text", MethodType.Setter)]
	private class Text
	{
		private static void Prefix(ref string value)
		{
			if (value.Trim().Length < 1 || value.Contains("<sprite="))
			{
				return;
			}
			TrippyModifiers? trippyModifiers = Service.Get<IModMenuService>().Get<TrippyModifiers>();
			if (trippyModifiers != null && trippyModifiers.ScrabbleEnabled)
			{
				string[] array = (from x in value.Split(' ', '\n', ',', '.', '-', '(', ')', '"', '*')
					where !string.IsNullOrWhiteSpace(x)
					select x).ToArray();
				_words.UnionWith(array);
				value = "";
				for (int num = 0; num < array.Length; num++)
				{
					value = value + _words.ElementAt(Random.Range(0, _words.Count)) + " ";
				}
				value = value.Trim();
			}
		}
	}

	private static readonly HashSet<string> _words = new HashSet<string>();
}
