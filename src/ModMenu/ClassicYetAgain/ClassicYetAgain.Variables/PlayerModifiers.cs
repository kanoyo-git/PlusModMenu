using System.Collections;
using System.Linq;
using System.Runtime.Serialization;
using ClassicYetAgain.Variables.Menus;
using PlusModMenu.Core.Common;
using PlusModMenu.Core.Common.Extensions;
using PlusModMenu.Core.Unity;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ClassicYetAgain.Variables;

[DataContract]
public class PlayerModifiers : IModifiers
{
	private class JumpManager
	{
		private readonly TaskManager.Task _jumpAnywhereTask;

		private bool _initialized;

		private float _initVelocity;

		private float _accel;

		private float _speedModifier;

		private float _maxHeight;

		public CameraModifier CameraModifier { get; set; }

		public JumpManager()
		{
			_jumpAnywhereTask = new TaskManager.Task(JumpRoutine);
		}

		public void Initialize()
		{
			Playtime obj = Resources.FindObjectsOfTypeAll<Playtime>().FirstOrDefault();
			Jumprope value = ((obj != null) ? AccessTools.Field(typeof(Playtime), "jumpropePre").GetValue<Jumprope>(obj) : null);
			if (value == null)
			{
				Debugger.Warning("JumpManager: Playtime's jumprope prefab couldn't be found, \"Jump Anywhere\" won't work.");
				return;
			}
			_initVelocity = AccessTools.Field(typeof(Jumprope), "initVelocity").GetValue<float>(value);
			_accel = AccessTools.Field(typeof(Jumprope), "accel").GetValue<float>(value);
			_speedModifier = 1f;
			_maxHeight = value.maxHeight;
			_initialized = true;
		}

		public void Start()
		{
			_jumpAnywhereTask.Start(resumeIfAlreadyRunning: true);
		}

		public void Stop()
		{
			_jumpAnywhereTask.Pause();
			CameraModifier.posOffset = Vector3.zero;
		}

		private static bool IsControllable()
		{
			GameCamera gameCamera = ((Singleton<CoreGameManager>.Instance != null) ? Singleton<CoreGameManager>.Instance.GetCamera(0) : null);
			if (!(gameCamera == null))
			{
				return gameCamera.Controllable;
			}
			return false;
		}

		private IEnumerator JumpRoutine()
		{
			while (!_initialized)
			{
				yield return null;
			}
			PlaytimeModifiers playtimeModifiers = Service.Get<IModMenuService>().Get<PlaytimeModifiers>();
			PlayerManager player = Singleton<CoreGameManager>.Instance.GetPlayer(0);
			Service.Get<IEventService>().Subscribe<IPlayerInstantiatedEvent>((object? _) =>
			{
				player = Singleton<CoreGameManager>.Instance.GetPlayer(0);
			});
			while (true)
			{
				if (!Input.GetKeyDown(ModMenu.BaldiPlusEngineGame.Config.Instance.JumpKey) || ModMenu.BaldiPlusEngineGame.Clipboard.ClipboardMenu.BlocksGame || Time.timeScale == 0f || !IsControllable() || player.jumpropes.Count > 0 || ModMenu.BaldiPlusEngineGame.Hl1.Hl1Mode.Active)
				{
					yield return null;
					continue;
				}
				float velocity = _initVelocity;
				while (CameraModifier.posOffset.y >= 0f && IsControllable())
				{
					CameraModifier.posOffset.y += (velocity * Time.deltaTime + 0.5f * _accel * Time.deltaTime * Time.deltaTime * _speedModifier) * playtimeModifiers.JumpHeightMultiplier;
					velocity += _accel * Time.deltaTime * _speedModifier;
					if (CameraModifier.posOffset.y > _maxHeight)
					{
						_maxHeight = CameraModifier.posOffset.y;
					}
					if (CameraModifier.posOffset.y <= 0f)
					{
						break;
					}
					yield return null;
				}
				CameraModifier.posOffset.y = 0f;
				yield return null;
			}
		}
	}

	private readonly CameraModifier _cameraModifier = new CameraModifier(Vector3.zero, Vector3.zero);

	private readonly JumpManager _jumpManager = new JumpManager();

	private bool _fliparoo;

	private bool _jumpAnywhere;

	[DataMember]
	[Label<Strings>("FLIPAROO")]
	[Help<HelpText>("FLIPAROO_TITLE", "FLIPAROO_TEXT")]
	[Group<PlayerModifiersOtherGroup>]
	public virtual bool Fliparoo
	{
		get
		{
			return _fliparoo;
		}
		set
		{
			_fliparoo = value;
			_cameraModifier.rotOffset.z = (value ? 180 : 0);
		}
	}

	[DataMember]
	[Label<Strings>("JUMP_ANYWHERE")]
	[Help<HelpText>("JUMP_ANYWHERE_TITLE", "JUMP_ANYWHERE_TEXT")]
	[Group<PlayerModifiersOtherGroup>]
	public virtual bool JumpAnywhere
	{
		get
		{
			return _jumpAnywhere;
		}
		set
		{
			_jumpAnywhere = value;
			if (value)
			{
				_jumpManager.Start();
			}
			else
			{
				_jumpManager.Stop();
			}
		}
	}

	protected PlayerModifiers()
	{
		_jumpManager.CameraModifier = _cameraModifier;
		Service.Get<IEventService>().Subscribe<IPlayerInstantiatedEvent>((object? _) =>
		{
			Singleton<CoreGameManager>.Instance.GetCamera(0).cameraModifiers.Add(_cameraModifier);
			Singleton<CoreGameManager>.Instance.GetPlayer(0).camMods.Add(_cameraModifier);
			_jumpManager.Initialize();
			if (Fliparoo)
			{
				Fliparoo = true;
			}
		});
	}
}
