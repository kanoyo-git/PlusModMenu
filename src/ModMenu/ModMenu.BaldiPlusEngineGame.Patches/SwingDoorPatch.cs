using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class SwingDoorPatch
{
	[HarmonyPatch(typeof(SwingDoor), "InsertItem")]
	private class InsertItem
	{
		private static bool Prefix(SwingDoor __instance)
		{
			__instance.LockTimed(Service.Get<IModMenuService>().Get<SwingDoorLockModifiers>().LockTime);
			return false;
		}
	}
}
