using HarmonyLib;
using MidiPlayerTK;
using ModMenu.BaldiPlusEngineGame.Variables.OtherModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class MidiFilePlayerPatch
{
	[HarmonyPatch(typeof(MidiFilePlayer), "MPTK_Speed", MethodType.Setter)]
	private class MPTK_Speed
	{
		private static bool Prefix(out bool __state)
		{
			AudioModifiers audioModifiers = Service.Get<IModMenuService>().Get<AudioModifiers>();
			return __state = !audioModifiers.LockSpeed || audioModifiers.BypassLock;
		}

		private static void Postfix(float value, bool __state)
		{
			if (__state)
			{
				Service.Get<IModMenuService>().Get<AudioModifiers>().UpdateControl("Speed", value);
			}
		}
	}

	[HarmonyPatch(typeof(MidiFilePlayer), "MPTK_Loop", MethodType.Setter)]
	private class MPTK_Loop
	{
		private static bool Prefix(out bool __state)
		{
			AudioModifiers audioModifiers = Service.Get<IModMenuService>().Get<AudioModifiers>();
			return __state = !audioModifiers.LockLoop || audioModifiers.BypassLock;
		}

		private static void Postfix(bool value, bool __state)
		{
			if (__state)
			{
				Service.Get<IModMenuService>().Get<AudioModifiers>().UpdateControl("Loop", value);
			}
		}
	}
}
