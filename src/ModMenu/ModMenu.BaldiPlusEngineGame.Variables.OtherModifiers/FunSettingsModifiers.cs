using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Patches;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables.OtherModifiers;

[DataContract]
internal class FunSettingsModifiers : IModifiers
{
	[DataMember(Name = "LightsOutEnabled")]
	private bool _enforceLightsOut;

	[DataMember(Name = "MirrorModeEnabled")]
	private bool _enforceMirrorMode;

	[Label<Strings>("LIGHTS_OUT")]
	[Help<HelpText>("LIGHTS_OUT_TITLE", "LIGHTS_OUT_TEXT")]
	[Group<SideMenuGroupOtherModifiersFunSettingsGeneral>]
	public virtual bool LightsOutEnabled
	{
		get
		{
			if (!_enforceLightsOut)
			{
				return LanternModePatch.LanternModeHelper.Instance?.Enabled ?? false;
			}
			return true;
		}
		set
		{
			_enforceLightsOut = value;
			SetLightsOut(value, CurrentEnvironment());
		}
	}

	[Label<Strings>("MIRROR_MODE")]
	[Help<HelpText>("MIRROR_MODE_TITLE", "MIRROR_MODE_TEXT")]
	[Group<SideMenuGroupOtherModifiersFunSettingsGeneral>]
	public virtual bool MirrorModeEnabled
	{
		get
		{
			if (!_enforceMirrorMode)
			{
				return MirrorModePatch.MirrorModeHelper.Instance?.Enabled ?? false;
			}
			return true;
		}
		set
		{
			_enforceMirrorMode = value;
			SetMirrorMode(value, CurrentEnvironment());
		}
	}

	public FunSettingsModifiers()
	{
		CoreGameManagerPatch.PlayersSpawned += (EnvironmentController ec) =>
		{
			if (_enforceLightsOut)
			{
				SetLightsOut(value: true, ec);
			}
			if (_enforceMirrorMode)
			{
				SetMirrorMode(value: true, ec);
			}
		};
	}

	[OnDeserialized]
	internal void OnDeserializedMethod(StreamingContext context)
	{
		if (_enforceLightsOut)
		{
			LightsOutEnabled = true;
		}
		if (_enforceMirrorMode)
		{
			MirrorModeEnabled = true;
		}
	}

	private static EnvironmentController? CurrentEnvironment()
	{
		if (!(Singleton<BaseGameManager>.Instance == null))
		{
			return Singleton<BaseGameManager>.Instance.Ec;
		}
		return null;
	}

	private static PlayerManager? GetPlayer()
	{
		if (!(Singleton<CoreGameManager>.Instance == null))
		{
			return Singleton<CoreGameManager>.Instance.GetPlayer(0);
		}
		return null;
	}

	private static void SetLightsOut(bool value, EnvironmentController? ec)
	{
		if (LanternModePatch.LanternModeHelper.Instance == null)
		{
			PlayerManager? player = GetPlayer();
			if (!value || ec == null || player == null)
			{
				return;
			}
			LanternMode lanternMode = LanternModePatch.LanternModeHelper.CreateLanternMode(ec.gameObject);
			lanternMode.Initialize(ec);
			lanternMode.AddSource(player.transform, 6f, new Color(0.887f, 0.765f, 0.498f, 1f));
		}
		if (!(LanternModePatch.LanternModeHelper.Instance == null))
		{
			LanternModePatch.LanternModeHelper.Instance.Enabled = value;
		}
	}

	private static void SetMirrorMode(bool value, EnvironmentController? ec)
	{
		if (MirrorModePatch.MirrorModeHelper.Instance == null)
		{
			if (!value || ec == null || GetPlayer() == null || Singleton<CoreGameManager>.Instance.GetCamera(0) == null)
			{
				return;
			}
			ec.gameObject.AddComponent<MirrorMode>().Initialize();
		}
		if (!(MirrorModePatch.MirrorModeHelper.Instance == null))
		{
			MirrorModePatch.MirrorModeHelper.Instance.Enabled = value;
		}
	}
}
