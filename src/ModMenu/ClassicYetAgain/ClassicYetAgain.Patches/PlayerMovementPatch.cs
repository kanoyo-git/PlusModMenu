using ClassicYetAgain.Variables;
using HarmonyLib;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ClassicYetAgain.Patches;

internal class PlayerMovementPatch
{
	[HarmonyPriority(Priority.Low)]
	[HarmonyPatch(typeof(PlayerMovement), "MouseMove")]
	private class MouseMove
	{
		private static void Prefix(PlayerMovement __instance, out bool __state)
		{
			__state = InvertOnFliparoo(__instance);
		}

		private static void Finalizer(PlayerMovement __instance, bool __state)
		{
			Restore(__instance, __state);
		}
	}

	[HarmonyPriority(Priority.Low)]
	[HarmonyPatch(typeof(PlayerMovement), "PlayerMove")]
	private class PlayerMove
	{
		private static void Prefix(PlayerMovement __instance, out bool __state)
		{
			__state = InvertOnFliparoo(__instance);
		}

		private static void Finalizer(PlayerMovement __instance, bool __state)
		{
			Restore(__instance, __state);
		}
	}

	private static bool InvertOnFliparoo(PlayerMovement instance)
	{
		if (instance.pm == null || !Service.Get<IModMenuService>().Get<PlayerModifiers>().Fliparoo)
		{
			return false;
		}
		instance.pm.reversed = !instance.pm.reversed;
		return true;
	}

	private static void Restore(PlayerMovement instance, bool inverted)
	{
		if (inverted && instance.pm != null)
		{
			instance.pm.reversed = !instance.pm.reversed;
		}
	}
}
