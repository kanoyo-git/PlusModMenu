using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class FloodEventPatch
{
	[HarmonyPatch(typeof(FloodEvent), "Initialize")]
	private class Initialize
	{
		private static void Prefix(ref float ___height, ref float ___riseSpeed, ref int ___minWhirlpools, ref int ___maxWhirlpools, ref MovementModifier ___moveMod)
		{
			FloodEventModifiers floodEventModifiers = Service.Get<IModMenuService>().Get<FloodEventModifiers>();
			___height = floodEventModifiers.MaxHeight;
			___riseSpeed = floodEventModifiers.RiseSpeed;
			___minWhirlpools = floodEventModifiers.MinWhirlpools;
			___maxWhirlpools = floodEventModifiers.MaxWhirlpools;
			___moveMod.movementMultiplier = floodEventModifiers.MovementMultiplier;
		}
	}
}
