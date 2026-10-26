using ClassicYetAgain.Variables.CharacterModifiers;
using HarmonyLib;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ClassicYetAgain.Patches;

internal class ArtsAndCraftersPatch
{
	[HarmonyPriority(Priority.Low)]
	[HarmonyPatch(typeof(ArtsAndCrafters), "Jealous", MethodType.Getter)]
	private class PlayerInSight
	{
		private static void Postfix(ref bool __result)
		{
			ArtsAndCraftersModifiers artsAndCraftersModifiers = Service.Get<IModMenuService>().Get<ArtsAndCraftersModifiers>();
			if (artsAndCraftersModifiers.OverrideJealousy)
			{
				__result = Singleton<BaseGameManager>.Instance.FoundNotebooks >= artsAndCraftersModifiers.CustomJealousy;
			}
		}
	}
}
