using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Interfaces;
using ModMenu.BaldiPlusEngineGame.Patches;
using ModMenu.BaldiPlusEngineGame.Utils;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Characters;
using PlusModMenu.Api.Characters.Interfaces;
using PlusModMenu.Api.Interfaces;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;

[DataContract]
public class BeansModifiers : AbstractCharacterModifiers<Beans>, ICharacterCanMove, ICharacterModifiers, IModifiers, ICharacterCanPlayAudio, ICharacterCanSee, ICharacterHasPhysicalAppearance, ICharacterHasPlayerTrigger, ICharacterLanternModeModifiers, ILanternModeSourceModifiers
{
	private bool _isSilent;

	private float _scale = 1f;

	private bool _isInvisible;

	private bool _emitsLightDuringLightsOut;

	private Color _lightsOutLightColor = Color.white;

	private float _lightsOutStrength = 4f;

	[DataMember]
	[Label<Strings>("ENABLED")]
	[Help<HelpText>("ENABLED_TITLE", "ENABLED_TEXT")]
	[Group<CharacterModifiersBeansGeneralGroup>]
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
	[Group<CharacterModifiersBeansGeneralGroup>]
	public virtual float Speed { get; set; } = 1f;

	[DataMember]
	[Label<Strings>("SILENT")]
	[Help<HelpText>("SILENT_TITLE", "SILENT_TEXT")]
	[Group<CharacterModifiersBeansGeneralGroup>]
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
	[Label<Strings>("BLIND")]
	[Help<HelpText>("BLIND_TITLE", "BLIND_TEXT")]
	[Group<CharacterModifiersBeansGeneralGroup>]
	public virtual bool IsBlind { get; set; }

	[DataMember]
	[Label<Strings>("SCALE")]
	[Help<HelpText>("SCALE_TITLE", "SCALE_TEXT")]
	[Group<CharacterModifiersBeansGeneralGroup>]
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
	[Group<CharacterModifiersBeansGeneralGroup>]
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
	[Group<CharacterModifiersBeansGeneralGroup>]
	public virtual bool IgnoresPlayerTrigger { get; set; }

	[DataMember]
	[Label<Strings>("COOLDOWN")]
	[Help<HelpText>("BEANS_COOLDOWN_TITLE", "BEANS_COOLDOWN_TEXT")]
	[Group<CharacterModifiersBeansGeneralGroup>]
	public virtual float Cooldown { get; set; } = 15f;

	[DataMember]
	[Label<Strings>("GUM_SPEED")]
	[Help<HelpText>("GUM_SPEED_TITLE", "GUM_SPEED_TEXT")]
	[Group<CharacterModifiersBeansGeneralGroup>]
	public virtual float GumSpeed { get; set; } = 30f;

	[DataMember]
	[Label<Strings>("GUM_STUCK_TIME")]
	[Help<HelpText>("GUM_STUCK_TIME_TITLE", "GUM_STUCK_TIME_TEXT")]
	[Group<CharacterModifiersBeansGeneralGroup>]
	public virtual float GumStuckTime { get; set; } = 10f;

	[DataMember]
	[Label<Strings>("EMITS_LIGHT_DURING_LIGHTS_OUT")]
	[Help<HelpText>("EMITS_LIGHT_DURING_LIGHTS_OUT_TITLE", "EMITS_LIGHT_DURING_LIGHTS_OUT_TEXT")]
	[Group<CharacterModifiersBeansLightsOutGroup>]
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
	[Group<CharacterModifiersBeansLightsOutGroup>]
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
	[Group<CharacterModifiersBeansLightsOutGroup>]
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

	[DataMember]
	[Label<Strings>("GUM_MOVEMENT_MULTIPLIER_PLAYER")]
	[Help<HelpText>("GUM_MOVEMENT_MULTIPLIER_TITLE", "GUM_MOVEMENT_MULTIPLIER_TEXT")]
	[Group<CharacterModifiersBeansGeneralGroup>]
	public virtual float GumMovementMultiplierPlayer { get; set; } = 0.25f;

	[DataMember]
	[Label<Strings>("GUM_MOVEMENT_MULTIPLIER_NPC")]
	[Help<HelpText>("GUM_MOVEMENT_MULTIPLIER_TITLE", "GUM_MOVEMENT_MULTIPLIER_TEXT")]
	[Group<CharacterModifiersBeansGeneralGroup>]
	public virtual float GumMovementMultiplierNPC { get; set; } = 0.05f;

	public BeansModifiers()
	{
		Helper.CreateCharacterGuiltMenu(Collection, typeof(CharacterModifiersBeansMenu));
		Helper.CreateCharacterInstanceMenu<Beans>(typeof(CharacterModifiersBeansMenu));
	}
}
