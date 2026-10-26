using System.Collections;
using HarmonyLib;
using ModMenu.Properties;
using TMPro;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

[HarmonyPatch(typeof(MainMenu), "Start")]
internal static class MainMenuLogoPatch
{
	public const string Name = "PlusModMenuVersion";

	public static string Text => $"Mod Menu+ {AssemblyInfo.VERSION}";

	private static void Postfix(MainMenu __instance)
	{
		__instance.StartCoroutine(Add(__instance));
	}

	private static IEnumerator Add(MainMenu menu)
	{
		yield return null;
		var reminder = menu.transform.Find("Reminder");
		var source = reminder != null ? reminder.GetComponent<TMP_Text>() : null;
		if (source == null || menu.transform.Find(Name) != null)
		{
			yield break;
		}
		var shown = source.gameObject.activeSelf && !string.IsNullOrWhiteSpace(source.text);
		var label = Object.Instantiate(source.gameObject, source.transform.parent).GetComponent<TMP_Text>();
		label.name = Name;
		label.gameObject.SetActive(true);
		foreach (var behaviour in label.GetComponents<MonoBehaviour>())
		{
			if (behaviour is not TMP_Text) Object.DestroyImmediate(behaviour);
		}
		label.raycastTarget = false;
		label.enableWordWrapping = false;
		label.text = Text;
		var position = source.transform.localPosition;
		if (shown)
		{
			source.ForceMeshUpdate();
			position.y -= source.preferredHeight + 1f;
		}
		label.transform.localPosition = position;
	}

	internal static (TMP_Text? label, TMP_Text? above) Find()
	{
		var menu = Object.FindObjectOfType<MainMenu>();
		if (menu == null) return (null, null);
		var label = menu.transform.Find(Name)?.GetComponent<TMP_Text>();
		var reminder = menu.transform.Find("Reminder")?.GetComponent<TMP_Text>();
		return (label, reminder != null && reminder.gameObject.activeSelf ? reminder : null);
	}
}
