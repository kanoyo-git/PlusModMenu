using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Collectors;
using PlusModMenu.Api.Collectors;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class NotebookPatch
{
	[HarmonyPatch(typeof(EnvironmentController), nameof(EnvironmentController.AddNotebook))]
	private class AddNotebook
	{
		private static void Postfix(Notebook book)
		{
			if (book != null)
			{
				MonoBehaviourCollector<Notebook, NotebookCollectorItem>.Add(book);
			}
		}
	}

	[HarmonyPatch(typeof(Notebook), "Start")]
	private class Start
	{
		private static void Prefix(Notebook __instance)
		{
			MonoBehaviourCollector<Notebook, NotebookCollectorItem>.Add(__instance);
		}
	}
}
