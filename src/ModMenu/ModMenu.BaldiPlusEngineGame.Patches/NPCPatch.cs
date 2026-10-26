using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PlusModMenu.Core.Unity;
using PlusModMenu.Core.Unity.Extensions;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.CustomComponents;
using ModMenu.BaldiPlusEngineGame.Utils;
using ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Characters;
using PlusModMenu.Api.Characters.Interfaces;
using PlusModMenu.Api.Collectors;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class NPCPatch
{
	[HarmonyPatch(typeof(NPC), MethodType.Constructor)]
	private class Constructor
	{
		private static void Postfix(NPC __instance)
		{
			UnityThread.ExecuteCoroutine(__instance.WaitForInitialization((NPC x) =>
			{
				if (!(x.gameObject == null) && x.gameObject.scene.IsGameScene())
				{
					IModifiers modifiers = Service.Get<ICharacterHelperService>().GetCharacterModifiersOfNPC(x.GetType()).FirstOrDefault((IModifiers y) => y is ICharacterCanPlayAudio characterCanPlayAudio2 && characterCanPlayAudio2 is ICharacterHasPhysicalAppearance);
					bool state;
					if (modifiers is ICharacterCanPlayAudio { IsSilent: not false })
					{
						CharacterUtils.SetSilence(out state, value: true, __instance);
					}
					if (modifiers is ICharacterHasPhysicalAppearance { IsInvisible: not false })
					{
						CharacterUtils.SetInvisible(out state, value: true, __instance);
					}
					GPSText componentInChildren = x.gameObject.GetComponentInChildren<GPSText>();
					if (componentInChildren != null)
					{
						UnityEngine.Object.Destroy(componentInChildren.gameObject);
					}
					GameObject gameObject = new GameObject("GPSText");
					gameObject.transform.parent = x.transform;
					gameObject.transform.localPosition = new Vector3(0f, -6f, 0f);
					gameObject.AddComponent<GPSText>().Text = Helper.GetCharacterName(x.GetType());
					if (Service.Get<ICharacterHelperService>().GetSpecificCharacterModifiersOfNPC(typeof(ICharacterHasPhysicalAppearance), x.GetType()) is ICharacterHasPhysicalAppearance characterHasPhysicalAppearance2)
					{
						CharacterUtils.SetScale(out var _, characterHasPhysicalAppearance2.Scale, __instance);
					}
					MonoBehaviourCollector<NPC, CharacterCollectorItem>.Add(x);
				}
			}));
		}
	}

	[HarmonyPatch]
	private class Hear
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			Type[] parameters = new Type[3]
			{
				typeof(GameObject),
				typeof(Vector3),
				typeof(int)
			};
			return from type in AccessTools.GetTypesFromAssembly(typeof(NPC).Assembly)
				where typeof(NPC).IsAssignableFrom(type) || typeof(NpcState).IsAssignableFrom(type)
				let method = type.GetMethod("Hear", AccessTools.allDeclared, null, parameters, null)
				where method != null && !method.IsAbstract
				select (MethodBase)method;
		}

		private static bool Prefix(object __instance)
		{
			NPC nPC = __instance as NPC ?? (__instance as NpcState)?.Npc;
			if (nPC == null)
			{
				return true;
			}
			return !(Service.Get<ICharacterHelperService>().GetSpecificCharacterModifiersOfNPC(typeof(ICharacterCanHear), nPC.GetType()) is ICharacterCanHear characterCanHear) || !characterCanHear.IsDeaf;
		}
	}

	[HarmonyPatch(typeof(NPC), "SetGuilt")]
	private class SetGuilt
	{
		private static bool Prefix(NPC __instance, string rule)
		{
			bool num = !Service.Get<IModMenuService>().Get<PrincipalModifiers>().Rules.TryGetValue(rule, out var value) | value;
			if (!num && string.Equals(rule, __instance.BrokenRule, StringComparison.Ordinal))
			{
				__instance.ClearGuilt();
			}
			return num;
		}
	}
}
