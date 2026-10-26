using HarmonyLib;
using MidiPlayerTK;
using ModMenu.BaldiPlusEngineGame.Variables.OtherModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class MidiSynthPatch
{
	[HarmonyPatch(typeof(MidiSynth), "MPTK_Transpose", MethodType.Setter)]
	private class MPTK_Transpose
	{
		private static bool Prefix(out bool __state)
		{
			AudioModifiers audioModifiers = Service.Get<IModMenuService>().Get<AudioModifiers>();
			return __state = !audioModifiers.LockPitch || audioModifiers.BypassLock;
		}

		private static void Postfix(int value, bool __state)
		{
			if (__state)
			{
				Service.Get<IModMenuService>().Get<AudioModifiers>().UpdateControl("Pitch", value);
			}
		}
	}
}
