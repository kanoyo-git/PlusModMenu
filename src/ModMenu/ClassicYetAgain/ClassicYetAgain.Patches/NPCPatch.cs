using System.Linq;
using ClassicYetAgain.BaldiClassicRemastered;
using HarmonyLib;
using UnityEngine;

namespace ClassicYetAgain.Patches;

internal class NPCPatch
{
	[HarmonyPriority(Priority.Low)]
	[HarmonyPatch(typeof(NPC), "Initialize")]
	private class Awake
	{
		private static void Postfix(NPC __instance)
		{
			if (__instance.GetComponentInChildren<LethalTouch>(includeInactive: true) != null)
			{
				return;
			}
			LethalTouch lethalTouch;
			if (__instance.gameObject.GetComponentsInChildren<Collider>().FirstOrDefault((Collider x) => x.isTrigger && x.enabled) == null)
			{
				GameObject gameObject = new GameObject("Lethal Touch", typeof(SphereCollider));
				gameObject.transform.SetParent(__instance.transform);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.GetComponent<SphereCollider>().isTrigger = true;
				lethalTouch = gameObject.AddComponent<LethalTouch>();
			}
			else
			{
				lethalTouch = __instance.gameObject.AddComponent<LethalTouch>();
			}
			lethalTouch.SetValues(__instance.GetType(), __instance);
		}
	}
}
