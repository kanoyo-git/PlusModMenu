using System;
using System.Collections;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using PlusModMenu.Core.Common;
using PlusModMenu.Core.Common.Utils;
using PlusModMenu.Core.Unity;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame;
using ModMenu.BaldiPlusEngineGame.Services;
using ModMenu.BaldiPlusEngineGame.Settings;
using ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;
using ModMenu.Properties;
using ModMenu.Settings;
using ModMenu.ReplacementNotice;
using PlusModMenu.Api;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces.Services;
using PlusModMenu.Api.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ModMenu.BepInEx;

[BepInPlugin(AssemblyInfo.PLUGIN_GUID, AssemblyInfo.PRODUCT, AssemblyInfo.VERSION)]
public class Plugin : BaseUnityPlugin
{
	public static Harmony Instance { get; } = new Harmony(AssemblyInfo.PLUGIN_GUID);

	private static string[] _leftovers = new string[0];

	private void Awake()
	{
		if (!IsGameValid())
		{
			FullscreenNotice.Create("<color='#ff0000'>ERROR</color>\r\nThis version of the <b>ModMenu</b> is intended to be used with <b>Baldi's Basics Plus</b> only.");
			return;
		}
		PlusModMenu.Hooks.BepInEx.Patcher.Apply();
		FindLeftovers();
		InitializeEarlyServices();
		InitializeGeneral(Config);
		InitializeLateServices();
		try
		{
			foreach (Type type in AccessTools.GetTypesFromAssembly(typeof(Plugin).Assembly).Where(x => !PlusModMenu.Hooks.BepInEx.Patcher.IsHooksType(x)))
			{
				Instance.CreateClassProcessor(type).Patch();
			}
		}
		catch (Exception arg)
		{
			FullscreenNotice.Create($"<color='#ff0000'>ERROR</color>\r\nA critical error occured while trying to prepare the ModMenu.\r\n\r\n{arg}").Alignment = TextAnchor.MiddleLeft;
			throw;
		}
		InputHelper._();
		UnityThread._();
		StartCoroutine(AwakeLate());
		if (AssemblyUtils.IsAssemblyDebugBuild(typeof(Baldi).Assembly))
		{
			gameObject.AddComponent<DebugWarner>();
		}
	}

	private static IEnumerator AwakeLate()
	{
		yield return BepInUtils.WaitForChainloader();
		PluginService pluginService = Service.Register(new PluginService());
		pluginService.LoadPlugins();
		Localization.Apply(ModMenu.BaldiPlusEngineGame.Config.Instance.MenuLanguage);
		ModMenuService modMenuService = (ModMenuService)Service.Get<IModMenuService>();
		modMenuService.Initialize();
		SettingsTreeBuilder.Build();
		modMenuService.GenerateInstances();
		Service.Get<IEventService>().Publish<IModMenuInitializedEvent>(null);
		ModMenu.BaldiPlusEngineGame.Config.GenerateUI();
		ModMenu.BaldiPlusEngineGame.Hl1.Hl1Mode.GenerateUI();
		ModMenu.BaldiPlusEngineGame.Hl1.GravityGunLevels.Init();
		HotkeyManager.Init();
		BindManager.Init();
		BulkModifiersManager.CreateBulkModifiers();
		KeptManager.Init();
		ModMenuManager.Create();
		if (_leftovers.Length > 0)
		{
			SettingsRegistryService.Registry.Dialogs.Message(UIText.Resource<Strings>("NOTICE"), UIText.Literal(string.Format(Strings.OLD_FILES_LEFT_TEXT, string.Join("\n", _leftovers))));
		}
	}

	private void FindLeftovers()
	{
		try
		{
			string plugins = Paths.PluginPath;
			string Relative(string path) => path.Substring(plugins.Length).TrimStart('/', '\\');
			_leftovers = LegacyAssemblies.Leftovers(Directory.GetFiles(plugins, "*", SearchOption.AllDirectories).Select(Relative), Relative(Info.Location)).ToArray();
		}
		catch (Exception e)
		{
			Logger.LogWarning("Could not check for old files: " + e.Message);
		}
		if (_leftovers.Length > 0)
		{
			Logger.LogError("Old Plus Mod Menu files in BepInEx/plugins, delete them: " + string.Join(", ", _leftovers));
		}
	}

	private static void InitializeGeneral(ConfigFile config)
	{
		Debugger.LogEvent += Debug.Log;
		Debugger.WarningEvent += Debug.LogWarning;
		Debugger.ErrorEvent += Debug.LogError;
		ContentPack.UnpackToMemory("content.tgz");
		DataPaths.Init("Kanoyo", "PlusModMenu");
		if (!File.Exists(config.ConfigFilePath))
		{
			MigrateLegacyData();
		}
		DataPaths.AddPath("Presets", "presets", RelativeTo.Mod);
		DataPaths.AddPath("Hotkeys", "hotkeys", RelativeTo.Mod);
		DataPaths.AddPath("SoundFonts", "soundfonts", RelativeTo.Mod);
		ModMenu.BaldiPlusEngineGame.Config.Init(config);
	}

	private static void MigrateLegacyData()
	{
		string legacyRoot = DataPaths.GetPath("Data") + "/Mods/" + new string(new char[] { 'F', 'a', 's', 'g', 'u', 'y' }) + "/ModMenu/";
		string newRoot = DataPaths.GetPath("Mod Directory");
		try
		{
			if (LegacyDataMigration.CopyIfEmpty(legacyRoot, newRoot))
			{
				Debug.Log("Migrated settings from " + legacyRoot + " to " + newRoot);
			}
		}
		catch (Exception e)
		{
			Debug.LogError("Could not migrate settings from " + legacyRoot + ": " + e);
		}
	}

	private static void InitializeEarlyServices()
	{
		IEventService eventService = Service.Register((IEventService)new EventService());
		SceneManager.sceneLoaded += (Scene scene, LoadSceneMode _) =>
		{
			eventService.Publish<ISceneChangedEvent>(scene);
		};
	}

	private static void InitializeLateServices()
	{
		Service.Register((ICharacterHelperService)new CharacterHelperService());
		Service.Register((IModMenuService)new ModMenuService());
		Service.Register((ISettingsRegistry)SettingsRegistryService.Registry);
	}

	private static bool IsGameValid()
	{
		return Application.temporaryCachePath.Contains("Basically Games/Baldi's Basics Plus");
	}
}
