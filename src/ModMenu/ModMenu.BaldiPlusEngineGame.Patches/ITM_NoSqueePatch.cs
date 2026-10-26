using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class ITM_NoSqueePatch
{
	[HarmonyPatch(typeof(ITM_NoSquee), "Use")]
	private class Use
	{
		private static void Prefix(ref float ___time, ref int ___distance)
		{
			WDNoSqueeModifiers wDNoSqueeModifiers = Service.Get<IModMenuService>().Get<WDNoSqueeModifiers>();
			___time = wDNoSqueeModifiers.Timer;
			___distance = wDNoSqueeModifiers.Radius;
		}
	}
}
