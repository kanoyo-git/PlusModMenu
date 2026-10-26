using System.Collections.Generic;
using System.Runtime.Serialization;
using ClassicYetAgain.Variables.Menus;
using ModMenu.BaldiPlusEngineGame.Settings;
using PlusModMenu.Api;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Collectors;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using PlusModMenu.Api.Settings;
using UnityEngine;

namespace ClassicYetAgain.Variables;

[DataContract]
public class CameraModifiers : IModifiers
{
	private readonly GameObject _cameraPositionDummy = new GameObject("Camera Position Dummy");

	private readonly InstanceCollector<PositionalData> _cameraPositions = new InstanceCollector<PositionalData>();

	private bool _cameraLocked;

	public CameraModifiers()
	{
		Object.DontDestroyOnLoad(_cameraPositionDummy);
		Service.Get<IEventService>().Subscribe<IPlayerInstantiatedEvent>((object? _) =>
		{
			_cameraLocked = false;
		});
		Service.Get<IEventService>().SubscribeOnce<IModMenuInitializedEvent>((object? _) =>
		{
			GenerateCameraManagerUI();
		});
	}

	[Label<Strings>("ADD_NEW_CAMERA")]
	[Group<SideMenuGroupCameraModifiersGeneralCameras>]
	public virtual void AddCamera()
	{
		if (!(Singleton<CoreGameManager>.Instance == null))
		{
			Transform transform = Singleton<CoreGameManager>.Instance.GetCamera(0).camCom.transform;
			_cameraPositions.Add(new PositionalData(transform.position, transform.rotation));
		}
	}

	[Label<Strings>("ENABLE_PLAYER_CAMERA")]
	[Help<HelpText>("ENABLE_PLAYER_CAMERA_TITLE", "ENABLE_PLAYER_CAMERA_TEXT")]
	[Group<SideMenuGroupCameraModifiersGeneralCameras>]
	public virtual void EnablePlayerCamera()
	{
		CoreGameManager instance = Singleton<CoreGameManager>.Instance;
		if (!(instance == null))
		{
			instance.GetCamera(0).UpdateTargets(null, 0);
			if (_cameraLocked)
			{
				_cameraLocked = false;
				instance.GetCamera(0).SetControllable(value: true);
			}
		}
	}

	private void GenerateCameraManagerUI()
	{
		List<ISettingHandle> buttons = new List<ISettingHandle>();
		ISettingsGroup group = SettingsRegistryService.Registry.GetGroup(typeof(SideMenuGroupCameraModifiersGeneralCameras));
		_cameraPositions.ItemAdded += (PositionalData _) =>
		{
			RefreshButtons();
		};
		_cameraPositions.ItemRemoved += (PositionalData _) =>
		{
			RefreshButtons();
		};
		RefreshButtons();
		void RefreshButtons()
		{
			foreach (ISettingHandle button in buttons)
			{
				button.Remove();
			}
			buttons.Clear();
			int index = 0;
			foreach (PositionalData positionalData in _cameraPositions.Collection)
			{
				index++;
				buttons.Add(group.AddButton(UIText.Resource<Strings>("CAMERA__INDEX", index), () =>
				{
					CoreGameManager instance = Singleton<CoreGameManager>.Instance;
					if (!(instance == null))
					{
						_cameraPositionDummy.transform.position = positionalData.Position;
						_cameraPositionDummy.transform.rotation = positionalData.Rotation;
						instance.GetCamera(0).UpdateTargets(_cameraPositionDummy.transform, 0);
						if (!_cameraLocked)
						{
							_cameraLocked = true;
							instance.GetCamera(0).SetControllable(value: false);
						}
					}
				}));
				buttons.Add(group.AddButton(UIText.Resource<Strings>("REMOVE_CAMERA__INDEX", index), () =>
				{
					_cameraPositions.Remove(positionalData);
				}));
			}
		}
	}
}
