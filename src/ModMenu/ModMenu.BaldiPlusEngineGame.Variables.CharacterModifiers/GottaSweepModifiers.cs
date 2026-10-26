using System.Reflection;
using System.Runtime.Serialization;
using PlusModMenu.Core.Common.Extensions;
using HarmonyLib;
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
public class GottaSweepModifiers : AbstractCharacterModifiers<GottaSweep>, ICharacterCanMove, ICharacterModifiers, IModifiers, ICharacterCanPlayAudio, ICharacterHasPhysicalAppearance, ICharacterHasPlayerTrigger, ICharacterLanternModeModifiers, ILanternModeSourceModifiers
{
	private static readonly FieldInfo _moveMod = AccessTools.Field(typeof(GottaSweep), "moveMod");

	private bool _isSilent;

	private float _scale = 1f;

	private bool _isInvisible;

	private float _sweepMovementMultiplier = 0.2f;

	private bool _emitsLightDuringLightsOut;

	private Color _lightsOutLightColor = Color.white;

	private float _lightsOutStrength = 4f;

	[DataMember]
	[Label<Strings>("ENABLED")]
	[Help<HelpText>("ENABLED_TITLE", "ENABLED_TEXT")]
	[Group<CharacterModifiersGottaSweepGeneralGroup>]
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
	[Group<CharacterModifiersGottaSweepGeneralGroup>]
	public virtual float Speed { get; set; } = 1f;

	[DataMember]
	[Label<Strings>("SILENT")]
	[Help<HelpText>("SILENT_TITLE", "SILENT_TEXT")]
	[Group<CharacterModifiersGottaSweepGeneralGroup>]
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
	[Group<CharacterModifiersGottaSweepGeneralGroup>]
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
	[Group<CharacterModifiersGottaSweepGeneralGroup>]
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
	[Group<CharacterModifiersGottaSweepGeneralGroup>]
	public virtual bool IgnoresPlayerTrigger { get; set; }

	[DataMember]
	[Label<Strings>("SWEEP_POWER_MULTIPLIER")]
	[Help<HelpText>("SWEEP_POWER_MULTIPLIER_TITLE", "SWEEP_POWER_MULTIPLIER_TEXT")]
	[Group<CharacterModifiersGottaSweepGeneralGroup>]
	public virtual float SweepPowerMultiplier { get; set; } = 1f;

	[DataMember]
	[Label<Strings>("SWEEP_MOVEMENT_MULTIPLIER")]
	[Help<HelpText>("SWEEP_MOVEMENT_MULTIPLIER_TITLE", "SWEEP_MOVEMENT_MULTIPLIER_TEXT")]
	[Group<CharacterModifiersGottaSweepGeneralGroup>]
	public virtual float SweepMovementMultiplier
	{
		get
		{
			return _sweepMovementMultiplier;
		}
		set
		{
			_sweepMovementMultiplier = value;
			foreach (CharacterCollectorItem item in Collection)
			{
				_moveMod.GetValue<MovementModifier>(item.Item).movementMultiplier = value;
			}
		}
	}

	[DataMember]
	[Label<Strings>("MIN_COOLDOWN")]
	[Help<HelpText>("GOTTA_SWEEP_MIN_COOLDOWN_TITLE", "GOTTA_SWEEP_MIN_COOLDOWN_TEXT")]
	[Group<CharacterModifiersGottaSweepGeneralGroup>]
	public virtual float MinCooldown { get; set; } = 60f;

	[DataMember]
	[Label<Strings>("MAX_COOLDOWN")]
	[Help<HelpText>("GOTTA_SWEEP_MAX_COOLDOWN_TITLE", "GOTTA_SWEEP_MAX_COOLDOWN_TEXT")]
	[Group<CharacterModifiersGottaSweepGeneralGroup>]
	public virtual float MaxCooldown { get; set; } = 180f;

	[DataMember]
	[Label<Strings>("MIN_TIME_ACTIVE")]
	[Help<HelpText>("MIN_TIME_ACTIVE_TITLE", "MIN_TIME_ACTIVE_TEXT")]
	[Group<CharacterModifiersGottaSweepGeneralGroup>]
	public virtual float MinActive { get; set; } = 45f;

	[DataMember]
	[Label<Strings>("MAX_TIME_ACTIVE")]
	[Help<HelpText>("MAX_TIME_ACTIVE_TITLE", "MAX_TIME_ACTIVE_TEXT")]
	[Group<CharacterModifiersGottaSweepGeneralGroup>]
	public virtual float MaxActive { get; set; } = 75f;

	[DataMember]
	[Label<Strings>("EMITS_LIGHT_DURING_LIGHTS_OUT")]
	[Help<HelpText>("EMITS_LIGHT_DURING_LIGHTS_OUT_TITLE", "EMITS_LIGHT_DURING_LIGHTS_OUT_TEXT")]
	[Group<CharacterModifiersGottaSweepLightsOutGroup>]
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
	[Group<CharacterModifiersGottaSweepLightsOutGroup>]
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
	[Group<CharacterModifiersGottaSweepLightsOutGroup>]
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

	public GottaSweepModifiers()
	{
		Helper.CreateCharacterGuiltMenu(Collection, typeof(CharacterModifiersGottaSweepMenu));
		Helper.CreateCharacterInstanceMenu<GottaSweep>(typeof(CharacterModifiersGottaSweepMenu));
	}
}
