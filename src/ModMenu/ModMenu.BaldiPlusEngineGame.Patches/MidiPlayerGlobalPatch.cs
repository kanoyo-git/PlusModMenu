using System.Linq;
using HarmonyLib;
using MidiPlayerTK;
using ModMenu.BaldiPlusEngineGame.Variables.OtherModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class MidiPlayerGlobalPatch
{
	[HarmonyPatch(typeof(MidiPlayerGlobal), "BuildMidiList")]
	private class BuildMidiList
	{
		private static void Postfix()
		{
			AudioModifiers audioModifiers = Service.Get<IModMenuService>().Get<AudioModifiers>();
			if (audioModifiers == null)
			{
				return;
			}
			foreach (string item in audioModifiers.MusicItems.OfType<string>().ToList())
			{
				audioModifiers.MusicItems.Remove(item);
			}
			foreach (string midiFile in MidiPlayerGlobal.CurrentMidiSet.MidiFiles)
			{
				audioModifiers.MusicItems.Add(midiFile);
			}
		}
	}
}
