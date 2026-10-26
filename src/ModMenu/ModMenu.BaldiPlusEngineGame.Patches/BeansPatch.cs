using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Patches.Helpers;
using ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class BeansPatch
{
	[HarmonyPatch(typeof(Beans))]
	private class AllMethods : ReplaceCoroutineCalls<Beans>
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			return ReplaceCoroutineCalls<Beans>._TargetMethods();
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return ReplaceCoroutineCalls<Beans>._Transpiler(instructions);
		}
	}

	[HarmonyPatch(typeof(Beans), "GumHit")]
	private class GumHit
	{
		private static void Prefix(ref float ___cooldownTime)
		{
			___cooldownTime = Service.Get<IModMenuService>().Get<BeansModifiers>().Cooldown;
		}
	}

	[HarmonyPatch(typeof(Beans), "Spit")]
	private class Spit
	{
		private static readonly FieldInfo _speed = AccessTools.Field(typeof(Gum), "speed");

		private static void Prefix(Beans __instance)
		{
			_speed.SetValue(__instance.gum, Service.Get<IModMenuService>().Get<BeansModifiers>().GumSpeed);
		}
	}
}
