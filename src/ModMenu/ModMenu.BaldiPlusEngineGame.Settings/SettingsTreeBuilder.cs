using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using PlusModMenu.Core.Common.Utils;
using ModMenu.BaldiPlusEngineGame.Services;
using ModMenu.Settings;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Settings;

internal static class SettingsTreeBuilder
{
	public static void Build()
	{
		var menus = new List<IStaticMenu>();
		var groups = new List<IStaticGroup>();
		foreach (Assembly assembly in Service.Get<PluginService>().AvailablePlugins.Select((IPlugin x) => x.GetType().Assembly))
		{
			var types = TypeUtils.GetLoadableTypes(assembly).ToList();
			menus.AddRange(TypeUtils.GetFullTypesImplementing(types, typeof(IStaticMenu)).Select(x => (IStaticMenu)FormatterServices.GetUninitializedObject(x)));
			groups.AddRange(TypeUtils.GetFullTypesImplementing(types, typeof(IStaticGroup)).Select(x => (IStaticGroup)FormatterServices.GetUninitializedObject(x)));
		}
		SettingsRegistryService.Registry.Build(
			groups.Select(g => (g.GetType(), g.Label.ToUIText(), g.ParentMenu, g.GetType().GetCustomAttribute<OrderAttribute>())),
			menus.Select(m => (m.GetType(), m.Label.ToUIText(), m.ParentGroup, m.GetType().GetCustomAttribute<OrderAttribute>())));
	}
}
