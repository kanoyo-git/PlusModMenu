using System;
using PlusModMenu.Core.Unity;
using ModMenu.BaldiPlusEngineGame.Clipboard;
using ModMenu.BaldiPlusEngineGame.Patches;
using ModMenu.BaldiPlusEngineGame.Settings;
using ModMenu.BaldiPlusEngineGame.Variables;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using PlusModMenu.Api.Settings;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Hl1;

internal static class Hl1Mode
{
	private static readonly string[] Conflicting = { "Speed", "Height", "MaxStamina", "InfiniteStamina" };

	private static bool _subscribed;

	private static PlayerModifiers.PlayerCameraMode _cameraMode;

	private static bool _jumpAnywhere;

	private static float _speed = 1f;

	private static float _height;

	public static bool Active { get; private set; }

	public static Hl1Controller? Controller { get; private set; }

	public static Speedometer? Speedometer { get; private set; }

	public static CarryController? Carry { get; private set; }

	public static event Action<bool>? Changed;

	public static void Set(bool on)
	{
		if (on == Active)
		{
			return;
		}
		Subscribe();
		var player = Service.Get<IModMenuService>().Get<PlayerModifiers>();
		var classic = Service.Get<IModMenuService>().Get<ClassicYetAgain.Variables.PlayerModifiers>();
		Active = on;
		if (on)
		{
			_cameraMode = player.CameraMode;
			_jumpAnywhere = classic.JumpAnywhere;
			_speed = player.Speed;
			_height = player.Height;
			player.CameraMode = PlayerModifiers.PlayerCameraMode.Unlocked;
			classic.JumpAnywhere = true;
			player.Speed = 1f;
			player.Height = 0f;
			if (Singleton<CoreGameManager>.Instance?.GetPlayer(0) is PlayerManager pm && Singleton<BaseGameManager>.Instance?.Ec is EnvironmentController ec)
			{
				StartLevel(ec, pm);
			}
		}
		else
		{
			StopLevel();
			player.CameraMode = _cameraMode;
			classic.JumpAnywhere = _jumpAnywhere;
			if (player.Speed == 1f) player.Speed = _speed;
			if (player.Height == 0f) player.Height = _height;
		}
		var registry = SettingsRegistryService.Registry;
		foreach (var name in Conflicting)
		{
			registry.Find(player, name)?.SetAvailable(!on, UIText.Resource<Strings>("HL1_UNAVAILABLE"));
			registry.Refresh(player, name);
		}
		registry.Refresh(player, "CameraMode");
		registry.Refresh(classic, "JumpAnywhere");
		Changed?.Invoke(on);
	}

	public static void GenerateUI()
	{
		var registry = SettingsRegistryService.Registry;
		var group = registry.GetGroup(typeof(PlayerModifiersMovementGroup));
		KeyCapture.AddButton(group, "JUMP_KEY__KEY", () => Config.Instance.JumpKey, key => Config.Instance.JumpKey = key, mouse: true);
		KeyCapture.AddButton(group, "HL1_DUCK_KEY__KEY", () => Config.Instance.Hl1DuckKey, key => Config.Instance.Hl1DuckKey = key);
		KeyCapture.AddButton(group, "HL1_USE_KEY__KEY", () => Config.Instance.Hl1UseKey, key => Config.Instance.Hl1UseKey = key);
		KeyCapture.AddButton(registry.GetGroup(typeof(ClassicYetAgain.Variables.Menus.PlayerModifiersOtherGroup)), "JUMP_KEY__KEY", () => Config.Instance.JumpKey, key => Config.Instance.JumpKey = key, mouse: true);
	}

	private static void Subscribe()
	{
		if (_subscribed)
		{
			return;
		}
		_subscribed = true;
		CoreGameManagerPatch.PlayersSpawned += ec =>
		{
			if (Active && Singleton<CoreGameManager>.Instance.GetPlayer(0) is PlayerManager pm)
			{
				StartLevel(ec, pm);
			}
		};
	}

	private static void StartLevel(EnvironmentController ec, PlayerManager player)
	{
		StopLevel();
		LevelSolids.Build(ec);
		Controller = Hl1Controller.Attach(player);
		var controller = Controller;
		Carry = CarryController.Attach(player, controller);
		Speedometer = Speedometer.Create(player, () => controller != null ? controller.SpeedHl : 0f);
	}

	private static void StopLevel()
	{
		if (Carry != null)
		{
			Carry.Detach();
			Carry = null;
		}
		CarryController.RemoveCrates();
		if (Controller != null)
		{
			var controller = Controller;
			Controller = null;
			controller.Detach();
		}
		if (Speedometer != null)
		{
			Speedometer.Remove();
			Speedometer = null;
		}
		Hl1CollisionPatch.RestoreAll();
		LevelSolids.Clear();
	}
}
