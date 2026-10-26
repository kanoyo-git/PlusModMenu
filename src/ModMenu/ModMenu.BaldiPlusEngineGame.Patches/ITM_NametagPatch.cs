using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class ITM_NametagPatch
{
	[HarmonyPatch(typeof(ITM_Nametag), "Use")]
	private class Use
	{
		private static void Prefix(ref float ___setTime)
		{
			___setTime = Service.Get<IModMenuService>().Get<NametagModifiers>().Timer;
		}
	}
}
