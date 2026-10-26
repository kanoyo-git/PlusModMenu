using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class ITM_ZestyBarPatch
{
	[HarmonyPatch(typeof(ITM_ZestyBar), "Use")]
	private class Use
	{
		private static void Postfix(PlayerManager pm)
		{
			pm.plm.stamina = pm.plm.StaminaMax * Service.Get<IModMenuService>().Get<ZestyBarModifiers>().StaminaMultiplier;
		}
	}
}
