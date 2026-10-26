using PlusModMenu.Core.Unity.Extensions;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.CharacterHelpers;
using ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class ChalkboardPatch
{
	[HarmonyPatch(typeof(Chalkboard), "Spawn")]
	private class Spawn
	{
		private static void Postfix(Chalkboard __instance, Cell tile, ChalkFace chalkFace)
		{
			Direction direction = Direction.North;
			float num = float.MaxValue;
			for (int i = 0; i < 4; i++)
			{
				float num2 = Quaternion.Angle(__instance.Rotation, ((Direction)i).ToRotation());
				if (num2 < num)
				{
					num = num2;
					direction = (Direction)i;
				}
			}
			chalkFace.GetOrAddComponent<ChalklesHelper>().Add(__instance, tile, direction);
		}
	}

	[HarmonyPatch(typeof(Chalkboard), "OnPlayerEnter")]
	private class OnTriggerEnter
	{
		private static bool Prefix(Chalkboard __instance)
		{
			if (__instance.enabled)
			{
				return !Service.Get<IModMenuService>().Get<ChalklesModifiers>().IgnoresPlayerTrigger;
			}
			return false;
		}
	}
}
