using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Interfaces;
using ModMenu.BaldiPlusEngineGame.Patches;
using ModMenu.BaldiPlusEngineGame.Utils;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Characters;
using PlusModMenu.Api.Characters.Interfaces;
using PlusModMenu.Api.Interfaces;
using HarmonyLib;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;

[DataContract]
public class DrReflexModifiers : AbstractCharacterModifiers<DrReflex>, ICharacterCanMove, ICharacterModifiers, IModifiers, ICharacterCanPlayAudio, ICharacterHasPhysicalAppearance, ICharacterHasPlayerTrigger, ICharacterLanternModeModifiers, ILanternModeSourceModifiers
{
	private bool _isSilent;

	private float _scale = 1f;

	private bool _isInvisible;

	private bool _emitsLightDuringLightsOut;

	private Color _lightsOutLightColor = Color.white;

	private float _lightsOutStrength = 4f;

	private static readonly AccessTools.FieldRef<DrReflex, float> _cooldownField = AccessTools.FieldRefAccess<DrReflex, float>("cooldown");

	private static readonly AccessTools.FieldRef<DrReflex, int> _rewardField = AccessTools.FieldRefAccess<DrReflex, int>("initialYtps");

	private static readonly AccessTools.FieldRef<DrReflex, int> _penaltyField = AccessTools.FieldRefAccess<DrReflex, int>("ytpPenalty");

	private float _cooldown = 30f;

	private int _reward = 25;

	private int _penalty = 5;

	[DataMember]
	[Label<Strings>("ENABLED")]
	[Help<HelpText>("ENABLED_TITLE", "ENABLED_TEXT")]
	[Group<CharacterModifiersDrReflexGeneralGroup>]
	public override bool Enabled
	{
		get
		{
			return base.Enabled;
		}
		set
		{
			base.Enabled = value;
		}
	}

	[DataMember]
	[Label<Strings>("SPEED")]
	[Help<HelpText>("SPEED_TITLE", "SPEED_TEXT")]
	[Group<CharacterModifiersDrReflexGeneralGroup>]
	public virtual float Speed { get; set; } = 1f;

	[DataMember]
	[Label<Strings>("SILENT")]
	[Help<HelpText>("SILENT_TITLE", "SILENT_TEXT")]
	[Group<CharacterModifiersDrReflexGeneralGroup>]
	public virtual bool IsSilent
	{
		get
		{
			return _isSilent;
		}
		set
		{
			CharacterUtils.SetSilence(out _isSilent, value, Collection);
		}
	}

	[DataMember]
	[Label<Strings>("SCALE")]
	[Help<HelpText>("SCALE_TITLE", "SCALE_TEXT")]
	[Group<CharacterModifiersDrReflexGeneralGroup>]
	public virtual float Scale
	{
		get
		{
			return _scale;
		}
		set
		{
			CharacterUtils.SetScale(out _scale, value, Collection);
		}
	}

	[DataMember]
	[Label<Strings>("INVISIBLE")]
	[Help<HelpText>("INVISIBLE_TITLE", "INVISIBLE_TEXT")]
	[Group<CharacterModifiersDrReflexGeneralGroup>]
	public virtual bool IsInvisible
	{
		get
		{
			return _isInvisible;
		}
		set
		{
			CharacterUtils.SetInvisible(out _isInvisible, value, Collection);
		}
	}

	[DataMember]
	[Label<Strings>("IGNORES_PLAYER_TRIGGER")]
	[Help<HelpText>("IGNORES_PLAYER_TRIGGER_TITLE", "IGNORES_PLAYER_TRIGGER_TEXT")]
	[Group<CharacterModifiersDrReflexGeneralGroup>]
	public virtual bool IgnoresPlayerTrigger { get; set; }

	[DataMember]
	[Label<Strings>("COOLDOWN")]
	[Help<HelpText>("REFLEX_COOLDOWN_TITLE", "REFLEX_COOLDOWN_TEXT")]
	[Group<CharacterModifiersDrReflexGeneralGroup>]
	public virtual float Cooldown
	{
		get
		{
			return _cooldown;
		}
		set
		{
			_cooldown = value;
			ApplyAll();
		}
	}

	[DataMember]
	[Label<Strings>("REFLEX_REWARD")]
	[Help<HelpText>("REFLEX_REWARD_TITLE", "REFLEX_REWARD_TEXT")]
	[Group<CharacterModifiersDrReflexGeneralGroup>]
	public virtual int Reward
	{
		get
		{
			return _reward;
		}
		set
		{
			_reward = value;
			ApplyAll();
		}
	}

	[DataMember]
	[Label<Strings>("REFLEX_PENALTY")]
	[Help<HelpText>("REFLEX_PENALTY_TITLE", "REFLEX_PENALTY_TEXT")]
	[Group<CharacterModifiersDrReflexGeneralGroup>]
	public virtual int Penalty
	{
		get
		{
			return _penalty;
		}
		set
		{
			_penalty = value;
			ApplyAll();
		}
	}

	[DataMember]
	[Label<Strings>("EMITS_LIGHT_DURING_LIGHTS_OUT")]
	[Help<HelpText>("EMITS_LIGHT_DURING_LIGHTS_OUT_TITLE", "EMITS_LIGHT_DURING_LIGHTS_OUT_TEXT")]
	[Group<CharacterModifiersDrReflexLightsOutGroup>]
	public virtual bool EmitsLightDuringLightsOut
	{
		get
		{
			return _emitsLightDuringLightsOut;
		}
		set
		{
			_emitsLightDuringLightsOut = value;
			LanternModePatch.LanternModeHelper.SetEmitsLightDuringLightsOut(Collection, value, LightsOutStrength, LightsOutLightColor);
		}
	}

	[DataMember]
	[Label<Strings>("LIGHT_COLOR")]
	[Help<HelpText>("LIGHT_COLOR_TITLE", "LIGHT_COLOR_TEXT")]
	[Group<CharacterModifiersDrReflexLightsOutGroup>]
	public virtual Color LightsOutLightColor
	{
		get
		{
			return _lightsOutLightColor;
		}
		set
		{
			_lightsOutLightColor = value;
			LanternModePatch.LanternModeHelper.SetLightColor(Collection, value);
		}
	}

	[DataMember]
	[Label<Strings>("LIGHT_STRENGTH")]
	[Help<HelpText>("LIGHT_STRENGTH_TITLE", "LIGHT_STRENGTH_TEXT")]
	[Group<CharacterModifiersDrReflexLightsOutGroup>]
	public virtual float LightsOutStrength
	{
		get
		{
			return _lightsOutStrength;
		}
		set
		{
			_lightsOutStrength = value;
			LanternModePatch.LanternModeHelper.SetLightStrength(Collection, value);
		}
	}

	public DrReflexModifiers()
	{
		Helper.CreateCharacterGuiltMenu(Collection, typeof(CharacterModifiersDrReflexMenu));
		Helper.CreateCharacterInstanceMenu<DrReflex>(typeof(CharacterModifiersDrReflexMenu));
	}

	internal void Apply(DrReflex reflex)
	{
		_cooldownField(reflex) = _cooldown;
		_rewardField(reflex) = _reward;
		_penaltyField(reflex) = _penalty;
	}

	private void ApplyAll()
	{
		foreach (var item in Collection)
		{
			Apply((DrReflex)item.Item);
		}
	}

	[HarmonyPatch(typeof(DrReflex), nameof(DrReflex.Initialize))]
	private class InitializePatch
	{
		private static void Postfix(DrReflex __instance)
		{
			PlusModMenu.Api.Service.Get<PlusModMenu.Api.Interfaces.Services.IModMenuService>().Get<DrReflexModifiers>().Apply(__instance);
		}
	}
}
