using System;
using System.Linq;
using HarmonyLib;

namespace PlusModMenu.Hooks.BepInEx;

public static class Patcher
{
	private static Harmony? _harmony;

	public static bool IsHooksType(Type type)
	{
		return type.Namespace != null && (type.Namespace == "PlusModMenu.Hooks" || type.Namespace.StartsWith("PlusModMenu.Hooks.", StringComparison.Ordinal));
	}

	public static void Apply()
	{
		if (_harmony != null)
		{
			return;
		}
		_harmony = new Harmony(PlusModMenu.Hooks.Properties.AssemblyInfo.PLUGIN_GUID);
		foreach (Type type in AccessTools.GetTypesFromAssembly(typeof(Patcher).Assembly).Where(IsHooksType))
		{
			_harmony.CreateClassProcessor(type).Patch();
		}
	}
}
