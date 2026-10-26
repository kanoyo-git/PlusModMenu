using ClassicYetAgain.Variables;
using HarmonyLib;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ClassicYetAgain.Patches;

internal class SubtitleControllerPatch
{
	[HarmonyPriority(Priority.Low)]
	[HarmonyPatch(typeof(SubtitleController), "PositionSub")]
	private class PositionSub
	{
		private static void Postfix(SubtitleController __instance)
		{
			if (!(__instance.soundTran == null) && !(__instance.camTran == null) && !(__instance.rectTransform == null) && Service.Get<IModMenuService>().Get<PlayerModifiers>().Fliparoo)
			{
				Vector2 anchoredPosition = __instance.rectTransform.anchoredPosition;
				anchoredPosition.x *= -1f;
				__instance.rectTransform.anchoredPosition = anchoredPosition;
			}
		}
	}
}
