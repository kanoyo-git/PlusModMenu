using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class ITM_GrapplingHookPatch
{
	[HarmonyPatch(typeof(ITM_GrapplingHook), "Use")]
	private class Use
	{
		private static void Prefix(ref float ___forceIncrease, ref float ___initialForce, ref float ___maxPressure)
		{
			GrapplingHookModifiers grapplingHookModifiers = Service.Get<IModMenuService>().Get<GrapplingHookModifiers>();
			___forceIncrease = grapplingHookModifiers.VelocityIncrease;
			___initialForce = grapplingHookModifiers.VelocityStart;
			___maxPressure = grapplingHookModifiers.MaxPressure;
		}
	}
}
