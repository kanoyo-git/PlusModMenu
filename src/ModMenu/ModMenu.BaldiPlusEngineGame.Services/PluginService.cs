using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PlusModMenu.Core.Common;
using PlusModMenu.Core.Common.Utils;
using ModMenu.BaldiPlusEngineGame.Internal.Menus;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;
using UnityEngine.UI;

namespace ModMenu.BaldiPlusEngineGame.Services;

internal class PluginService : IService
{
	private bool _loaded;

	public IReadOnlyCollection<IPlugin> AvailablePlugins { get; private set; } = Array.Empty<IPlugin>();

	internal void LoadPlugins()
	{
		if (_loaded)
		{
			Debugger.Warning("Tried loading plugins after they were already loaded.");
			return;
		}
		HashSet<Assembly> hashSet = new HashSet<Assembly>();
		hashSet.Add(typeof(VanillaPlugin).Assembly);
		hashSet.UnionWith(AppDomain.CurrentDomain.GetAssemblies());
		List<IPlugin> list = new List<IPlugin>();
		foreach (Assembly item in hashSet)
		{
			try
			{
				Type[] array = (from x in TypeUtils.GetLoadableTypes(item)
					where typeof(IPlugin).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract
					select x).ToArray();
				int num = array.Length;
				if (num <= 0)
				{
					continue;
				}
				if (num > 1)
				{
					Debugger.Error("The Assembly '" + item.FullName + "' contains more than one ModMenu plugin, which is not allowed.");
					continue;
				}
				IPlugin plugin = (IPlugin)Activator.CreateInstance(array.First());
				if (string.IsNullOrWhiteSpace(plugin.Name))
				{
					Debugger.Error("No name has been provided for the ModMenuPlugin of assembly '" + item.FullName + "', which is not allowed.");
				}
				else
				{
					list.Add(plugin);
				}
			}
			catch (Exception arg)
			{
				Debugger.Error($"An error occured while trying to initialize a plugin.\n{arg}");
			}
		}
		AvailablePlugins = list.AsReadOnly();
		_loaded = true;
	}
}
