using System.Linq;
using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Hl1;
using ModMenu.BaldiPlusEngineGame.Patches;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using ModMenu.BaldiPlusEngineGame.Settings;
using ModMenu.Settings.Editors;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables;

[DataContract]
public class PlayerModifiers : IModifiers
{
	public enum PlayerCameraMode
	{
		[Label<Strings>("CAMERA_MODE_LOCKED")]
		Locked,
		[Label<Strings>("CAMERA_MODE_UNLOCKED")]
		Unlocked
	}

	private readonly MovementModifier _movementModifier = new MovementModifier(Vector3.zero, 1f);

	private readonly CameraModifier _cameraModifier = new CameraModifier(Vector3.zero, Vector3.zero);

	private float _maxStamina = 100f;

	private float _cameraFOV = 60f;

	private bool _emitsLightDuringLightsOut = true;

	private Color _lightsOutLightColor = new Color(0.887f, 0.765f, 0.498f);

	private float _lightsOutStrength = 6f;

	private bool _godMode;

	[DataMember]
	[Label<Strings>("GOD_MODE")]
	[Help<HelpText>("GOD_MODE_TITLE", "GOD_MODE_TEXT")]
	[Group<PlayerModifiersSurvivalGroup>]
	public virtual bool GodMode
	{
		get
		{
			return _godMode;
		}
		set
		{
			_godMode = value;
			var player = Singleton<CoreGameManager>.Instance != null ? Singleton<CoreGameManager>.Instance.GetPlayer(0) : null;
			if (player != null)
			{
				player.invincible = value;
			}
		}
	}

	[DataMember]
	[Label<Strings>("INFINITE_STAMINA")]
	[Help<HelpText>("INFINITE_STAMINA_TITLE", "INFINITE_STAMINA_TEXT")]
	[Group<PlayerModifiersMovementGroup>]
	public virtual bool InfiniteStamina { get; set; }

	[DataMember]
	[Label<Strings>("HL1_MOVEMENT")]
	[Help<HelpText>("HL1_MOVEMENT_TITLE", "HL1_MOVEMENT_TEXT")]
	[Group<PlayerModifiersMovementGroup>]
	public virtual bool Hl1Movement
	{
		get
		{
			return Hl1Mode.Active;
		}
		set
		{
			Hl1Mode.Set(value);
		}
	}

	[DataMember]
	[Label<Strings>("NOCLIP")]
	[Help<HelpText>("NOCLIP_TITLE", "NOCLIP_TEXT")]
	[Group<PlayerModifiersMovementGroup>]
	public virtual bool Noclip { get; set; }

	[DataMember]
	[Label<Strings>("SPEED")]
	[Help<HelpText>("SPEED_TITLE", "SPEED_TEXT")]
	[Group<PlayerModifiersMovementGroup>]
	public virtual float Speed
	{
		get
		{
			return _movementModifier.movementMultiplier;
		}
		set
		{
			_movementModifier.movementMultiplier = value;
		}
	}

	[DataMember]
	[Label<Strings>("MAX_STAMINA")]
	[Help<HelpText>("MAX_STAMINA_TITLE", "MAX_STAMINA_TEXT")]
	[Group<PlayerModifiersMovementGroup>]
	public virtual float MaxStamina
	{
		get
		{
			return _maxStamina;
		}
		set
		{
			PlayerMovement playerMovement = Singleton<CoreGameManager>.Instance?.GetPlayer(0).plm;
			if (!(playerMovement == null))
			{
				playerMovement.staminaMax = value;
				_maxStamina = value;
			}
		}
	}

	[DataMember]
	[Label<Strings>("CAMERA_FOV")]
	[Help<HelpText>("CAMERA_FOV_TITLE", "CAMERA_FOV_TEXT")]
	[Group<PlayerModifiersCameraGroup>]
	[OverrideEditorControl<RangeEditor>]
	[ValueRange(1f, 179f)]
	public virtual float CameraFOV
	{
		get
		{
			return _cameraFOV;
		}
		set
		{
			_cameraFOV = value;
			if (!(Singleton<CoreGameManager>.Instance == null))
			{
				GameCamera camera = Singleton<CoreGameManager>.Instance.GetCamera(0);
				if (!(camera == null))
				{
					float aspect = camera.camCom.aspect;
					float num = 1f / Mathf.Tan(value / 114.59156f);
					float num2 = num / aspect;
					Matrix4x4 projectionMatrix = camera.camCom.projectionMatrix;
					projectionMatrix[0, 0] = ((projectionMatrix[0, 0] < 0f) ? (0f - num2) : num2);
					projectionMatrix[1, 1] = num;
					camera.camCom.fieldOfView = value;
					camera.billboardCam.fieldOfView = value;
					camera.camCom.projectionMatrix = projectionMatrix;
					camera.billboardCam.projectionMatrix = projectionMatrix;
				}
			}
		}
	}

	[DataMember]
	[Label<Strings>("HEIGHT")]
	[Help<HelpText>("PLAYER_HEIGHT_TITLE", "PLAYER_HEIGHT_TEXT")]
	[Group<PlayerModifiersCameraGroup>]
	public virtual float Height
	{
		get
		{
			return _cameraModifier.posOffset.y;
		}
		set
		{
			_cameraModifier.posOffset.y = value;
		}
	}

	[DataMember]
	[Label<Strings>("CAMERA_MODE")]
	[Help<HelpText>("CAMERA_MODE_TITLE", "CAMERA_MODE_TEXT")]
	[Group<PlayerModifiersCameraGroup>]
	public virtual PlayerCameraMode CameraMode { get; set; }

	[DataMember]
	[Label<Strings>("EMITS_LIGHT_DURING_LIGHTS_OUT")]
	[Help<HelpText>("EMITS_LIGHT_DURING_LIGHTS_OUT_TITLE", "EMITS_LIGHT_DURING_LIGHTS_OUT_TEXT")]
	[Group<PlayerModifiersLightsOutGroup>]
	public virtual bool EmitsLightDuringLightsOut
	{
		get
		{
			return _emitsLightDuringLightsOut;
		}
		set
		{
			_emitsLightDuringLightsOut = value;
			LanternModePatch.LanternModeHelper instance = LanternModePatch.LanternModeHelper.Instance;
			if ((object)instance == null)
			{
				return;
			}
			LanternMode lanternMode = instance.LanternMode;
			if ((object)lanternMode == null)
			{
				return;
			}
			if (value)
			{
				lanternMode.AddSource(Singleton<CoreGameManager>.Instance.GetPlayer(0).transform, LightsOutStrength, LightsOutLightColor);
				return;
			}
			instance.Sources.RemoveAll((LanternSource x) => x.transform == Singleton<CoreGameManager>.Instance.GetPlayer(0).transform);
		}
	}

	[DataMember]
	[Label<Strings>("LIGHT_COLOR")]
	[Help<HelpText>("LIGHT_COLOR_TITLE", "LIGHT_COLOR_TEXT")]
	[Group<PlayerModifiersLightsOutGroup>]
	public virtual Color LightsOutLightColor
	{
		get
		{
			return _lightsOutLightColor;
		}
		set
		{
			_lightsOutLightColor = value;
			LanternModePatch.LanternModeHelper instance = LanternModePatch.LanternModeHelper.Instance;
			if ((object)instance == null || (object)instance.LanternMode == null)
			{
				return;
			}
			Transform playerTransform = Singleton<CoreGameManager>.Instance.GetPlayer(0).transform;
			foreach (LanternSource item in instance.Sources.Where((LanternSource x) => x.transform == playerTransform))
			{
				item.color = value;
			}
		}
	}

	[DataMember]
	[Label<Strings>("LIGHT_STRENGTH")]
	[Help<HelpText>("LIGHT_STRENGTH_TITLE", "LIGHT_STRENGTH_TEXT")]
	[Group<PlayerModifiersLightsOutGroup>]
	public virtual float LightsOutStrength
	{
		get
		{
			return _lightsOutStrength;
		}
		set
		{
			_lightsOutStrength = value;
			LanternModePatch.LanternModeHelper instance = LanternModePatch.LanternModeHelper.Instance;
			if ((object)instance == null || (object)instance.LanternMode == null)
			{
				return;
			}
			Transform playerTransform = Singleton<CoreGameManager>.Instance.GetPlayer(0).transform;
			foreach (LanternSource item in instance.Sources.Where((LanternSource x) => x.transform == playerTransform))
			{
				item.strength = value;
			}
		}
	}

	protected PlayerModifiers()
	{
		Service.Get<IEventService>().Subscribe<IPlayerInstantiatedEvent>((object? _) =>
		{
			PlayerMovement playerMovement = Object.FindObjectOfType<PlayerMovement>();
			if ((bool)playerMovement)
			{
				playerMovement.am.moveMods.Add(_movementModifier);
				CameraFOV = CameraFOV;
				GodMode = GodMode;
				Singleton<CoreGameManager>.Instance.GetCamera(0).cameraModifiers.Add(_cameraModifier);
				Singleton<CoreGameManager>.Instance.GetPlayer(0).camMods.Add(_cameraModifier);
			}
		});
	}
}
