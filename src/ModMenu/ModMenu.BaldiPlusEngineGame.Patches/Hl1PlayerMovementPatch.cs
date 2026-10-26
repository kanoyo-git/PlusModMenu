using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Hl1;
using ModMenu.BaldiPlusEngineGame.Variables;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

[HarmonyPatch(typeof(PlayerMovement), "PlayerMove")]
internal class Hl1PlayerMovementPatch
{
	[HarmonyPriority(Priority.First)]
	private static bool Prefix(PlayerMovement __instance)
	{
		var controller = Hl1Mode.Controller;
		if (!Hl1Mode.Active || controller == null || controller.gameObject != __instance.gameObject)
		{
			return true;
		}
		if (Service.Get<IModMenuService>().Get<PlayerModifiers>().Noclip)
		{
			controller.Suspend();
			Hl1CollisionPatch.Feet = null;
			return true;
		}
		controller.Step(__instance);
		return false;
	}
}
