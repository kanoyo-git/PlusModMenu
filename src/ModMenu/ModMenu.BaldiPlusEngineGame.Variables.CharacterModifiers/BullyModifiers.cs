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
public class BullyModifiers : AbstractCharacterModifiers<Bully>, ICharacterCanPlayAudio, ICharacterModifiers, IModifiers, ICharacterCanSee, ICharacterHasPhysicalAppearance, ICharacterHasPlayerTrigger, ICharacterLanternModeModifiers, ILanternModeSourceModifiers
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
	[Group<CharacterModifiersBullyGeneralGroup>]
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
	[Label<Strings>("SILENT")]
	[Help<HelpText>("SILENT_TITLE", "SILENT_TEXT")]
	[Group<CharacterModifiersBullyGeneralGroup>]
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
	[Group<CharacterModifiersBullyGeneralGroup>]
	public virtual bool IsBlind { get; set; }

	[DataMember]
	[Label<Strings>("SCALE")]
	[Help<HelpText>("SCALE_TITLE", "SCALE_TEXT")]
	[Group<CharacterModifiersBullyGeneralGroup>]
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
	[Group<CharacterModifiersBullyGeneralGroup>]
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
	[Group<CharacterModifiersBullyGeneralGroup>]
	public virtual bool IgnoresPlayerTrigger { get; set; }

	[DataMember]
	[Label<Strings>("MIN_COOLDOWN")]
	[Help<HelpText>("GOTTA_SWEEP_MIN_COOLDOWN_TITLE", "BULLY_MIN_COOLDOWN_TEXT")]
	[Group<CharacterModifiersBullyGeneralGroup>]
	public virtual float MinCooldown { get; set; } = 60f;

	[DataMember]
	[Label<Strings>("MAX_COOLDOWN")]
	[Help<HelpText>("GOTTA_SWEEP_MAX_COOLDOWN_TITLE", "BULLY_MAX_COOLDOWN_TEXT")]
	[Group<CharacterModifiersBullyGeneralGroup>]
	public virtual float MaxCooldown { get; set; } = 120f;

	[DataMember]
	[Label<Strings>("MAX_HALLTIME")]
	[Help<HelpText>("MAX_HALLTIME_TITLE", "MAX_HALLTIME_TEXT")]
	[Group<CharacterModifiersBullyGeneralGroup>]
	public virtual float MaxHalltime { get; set; } = 120f;

	[DataMember]
	[Label<Strings>("EMITS_LIGHT_DURING_LIGHTS_OUT")]
	[Help<HelpText>("EMITS_LIGHT_DURING_LIGHTS_OUT_TITLE", "EMITS_LIGHT_DURING_LIGHTS_OUT_TEXT")]
	[Group<CharacterModifiersBullyLightsOutGroup>]
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
	[Group<CharacterModifiersBullyLightsOutGroup>]
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
	[Group<CharacterModifiersBullyLightsOutGroup>]
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

	public BullyModifiers()
	{
		Helper.CreateCharacterGuiltMenu(Collection, typeof(CharacterModifiersBullyMenu));
		Helper.CreateCharacterInstanceMenu<Bully>(typeof(CharacterModifiersBullyMenu));
	}
}
