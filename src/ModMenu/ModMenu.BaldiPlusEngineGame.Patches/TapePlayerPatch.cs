using PlusModMenu.Core.Unity;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class TapePlayerPatch
{
	[HarmonyPatch(typeof(TapePlayer), "InsertItem")]
	private class InsertItem
	{
		private static void Prefix(TapePlayer __instance, ref float ___time, AudioManager ___audMan)
		{
			___time = Service.Get<IModMenuService>().Get<TapeModifiers>().AntiHearingTime;
			__instance.Invoke(() =>
			{
				___audMan.FlushQueue(endCurrent: true);
			}, Service.Get<IModMenuService>().Get<TapeModifiers>().AntiHearingTime);
		}
	}
}
