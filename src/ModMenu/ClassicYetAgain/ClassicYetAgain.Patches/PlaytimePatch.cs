using System.Linq;
using ClassicYetAgain.Collectors;
using ClassicYetAgain.Variables.CharacterModifiers;
using HarmonyLib;
using PlusModMenu.Api;
using PlusModMenu.Api.Collectors;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ClassicYetAgain.Patches;

internal class PlaytimePatch
{
	[HarmonyPriority(Priority.Low)]
	[HarmonyPatch(typeof(Playtime), "JumpropeHit")]
	private class JumpropeHit
	{
		private static bool Prefix(Playtime __instance)
		{
			if (!Service.Get<IModMenuService>().Get<PlaytimeModifiers>().Aggressive)
			{
				return true;
			}
			CoreGameManagerPatch.Helper.EndGame(Singleton<CoreGameManager>.Instance.GetPlayer(0).transform, __instance);
			return false;
		}
	}

	[HarmonyPriority(Priority.Low)]
	[HarmonyPatch(typeof(Playtime), "EndJumprope")]
	private class EndJumprope
	{
		private static void Postfix()
		{
			if (Service.Get<IModMenuService>().Get<PlaytimeModifiers>().DeliveryService)
			{
				ItemCollectorItem[] array = MonoBehaviourCollector<Pickup, ItemCollectorItem>.Collection.Where((ItemCollectorItem x) => x.Item.icon != null && x.Item.icon.spriteRenderer.enabled).ToArray();
				if (array.Length >= 1)
				{
					ItemManager itm = Singleton<CoreGameManager>.Instance.GetPlayer(0).itm;
					Pickup item = array[Random.Range(0, array.Length)].Item;
					itm.AddItem(item.item, item);
				}
			}
		}
	}
}
