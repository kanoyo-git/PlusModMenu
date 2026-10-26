using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using BepInEx.Bootstrap;
using ModMenu.Settings;
using PlusModMenu.Api.Settings;

namespace ModMenu.BaldiPlusEngineGame.Settings;

internal static class Localization
{
	private sealed class SwitchingManager : ResourceManager
	{
		private readonly ResourceManager _english;

		private readonly ResourceManager _russian;

		public SwitchingManager(string baseName, Assembly assembly)
		{
			_english = new ResourceManager(baseName, assembly);
			_russian = new LocalizedResourceManager(new ResourceManager(baseName + ".ru", assembly), _english);
		}

		private ResourceManager Current => Russian ? _russian : _english;

		public ResourceManager English => _english;

		public override string GetString(string name)
		{
			return Current.GetString(name, CultureInfo.InvariantCulture);
		}

		public override string GetString(string name, CultureInfo culture)
		{
			return Current.GetString(name, CultureInfo.InvariantCulture);
		}

		public override object GetObject(string name)
		{
			return Current.GetObject(name, CultureInfo.InvariantCulture);
		}

		public override object GetObject(string name, CultureInfo culture)
		{
			return Current.GetObject(name, CultureInfo.InvariantCulture);
		}
	}

	public static bool Russian { get; private set; }

	public static bool MissingCyrillic { get; private set; }

	public static event Action? Changed;

	public static void Apply(MenuLanguage setting)
	{
		var (russian, missingCyrillic) = LanguageResolver.Resolve(setting, Chainloader.PluginInfos.ContainsKey(LanguageResolver.CyrillicPlusGuid));
		var changed = russian != Russian || missingCyrillic != MissingCyrillic;
		Russian = russian;
		MissingCyrillic = missingCyrillic;
		if (changed)
		{
			Changed?.Invoke();
		}
	}

	public static string EnglishText(Type resourceType, string key)
	{
		var manager = PlusModMenu.Core.Common.ResourceHelper.ResourceManagerByType(resourceType);
		return (manager is SwitchingManager switching ? switching.English.GetString(key, CultureInfo.InvariantCulture) : manager.GetString(key, CultureInfo.InvariantCulture)) ?? key;
	}

	public static ResourceManager Manager(string baseName, Assembly assembly)
	{
		return new SwitchingManager(baseName, assembly);
	}
}
