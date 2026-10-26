using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class ChalkEraserPatch
{
	[HarmonyPatch(typeof(ChalkEraser), "Use")]
	private class Use
	{
		private static void Prefix(ref float ___setTime)
		{
			___setTime = Service.Get<IModMenuService>().Get<ChalkEraserModifiers>().Timer;
		}
	}
}
