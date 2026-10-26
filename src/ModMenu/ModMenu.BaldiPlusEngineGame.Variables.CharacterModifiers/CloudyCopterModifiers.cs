using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using PlusModMenu.Core.Common.Extensions;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.CharacterHelpers;
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
public class CloudyCopterModifiers : AbstractCharacterModifiers<Cumulo>, ICharacterCanMove, ICharacterModifiers, IModifiers, ICharacterCanPlayAudio, ICharacterHasPhysicalAppearance, ICharacterHasPlayerTrigger, ICharacterLanternModeModifiers, ILanternModeSourceModifiers
{
	private static readonly FieldInfo _windManager = AccessTools.Field(typeof(Cumulo), "windManager");

	private bool _isSilent;

	private float _scale = 1f;

	private bool _isInvisible;

	private float _windForceSpeed = 12f;

	private bool _emitsLightDuringLightsOut;

	private Color _lightsOutLightColor = Color.white;

	private float _lightsOutStrength = 4f;

	[DataMember]
	[Label<Strings>("ENABLED")]
	[Help<HelpText>("ENABLED_TITLE", "ENABLED_TEXT")]
	[Group<CharacterModifiersCloudyCopterGeneralGroup>]
	public override bool Enabled
	{
		get
		{
			return base.Enabled;
		}
		set
		{
			SubtitleController[] source = Resources.FindObjectsOfTypeAll<SubtitleController>();
			foreach (CharacterCollectorItem item in Collection)
			{
				AudioManager audioManager = _windManager.GetValue<BeltManager>(item.Item).GetComponentInChildren<AudioManager>();
				foreach (SubtitleController item2 in source.Where((SubtitleController x) => x.soundTran == audioManager.transform))
				{
					item2.gameObject.SetActive(value);
				}
				item.gameObject.SetActive(value);
			}
		}
	}

	[DataMember]
	[Label<Strings>("SPEED")]
	[Help<HelpText>("SPEED_TITLE", "SPEED_TEXT")]
	[Group<CharacterModifiersCloudyCopterGeneralGroup>]
	public virtual float Speed { get; set; } = 1f;

	[DataMember]
	[Label<Strings>("SILENT")]
	[Help<HelpText>("SILENT_TITLE", "SILENT_TEXT")]
	[Group<CharacterModifiersCloudyCopterGeneralGroup>]
	public virtual bool IsSilent
	{
		get
		{
			return _isSilent;
		}
		set
		{
			_isSilent = value;
			foreach (CharacterCollectorItem item in Collection)
			{
				CharacterUtils.SetSilence(out var _, value, _windManager.GetValue<BeltManager>(item.Item));
			}
		}
	}

	[DataMember]
	[Label<Strings>("SCALE")]
	[Help<HelpText>("SCALE_TITLE", "SCALE_TEXT")]
	[Group<CharacterModifiersCloudyCopterGeneralGroup>]
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
	[Group<CharacterModifiersCloudyCopterGeneralGroup>]
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
	[Group<CharacterModifiersCloudyCopterGeneralGroup>]
	public virtual bool IgnoresPlayerTrigger { get; set; }

	[DataMember]
	[Label<Strings>("MIN_STAY_TIME")]
	[Help<HelpText>("MIN_STAY_TIME_TITLE", "MIN_STAY_TIME_TEXT")]
	[Group<CharacterModifiersCloudyCopterGeneralGroup>]
	public virtual float MinStay { get; set; } = 10f;

	[DataMember]
	[Label<Strings>("MAX_STAY_TIME")]
	[Help<HelpText>("MAX_STAY_TIME_TITLE", "MAX_STAY_TIME_TEXT")]
	[Group<CharacterModifiersCloudyCopterGeneralGroup>]
	public virtual float MaxStay { get; set; } = 30f;

	[DataMember]
	[Label<Strings>("WIND_FORCE_SPEED")]
	[Help<HelpText>("WIND_FORCE_SPEED_TITLE", "WIND_FORCE_SPEED_TEXT")]
	[Group<CharacterModifiersCloudyCopterGeneralGroup>]
	public virtual float WindForceSpeed
	{
		get
		{
			return _windForceSpeed;
		}
		set
		{
			_windForceSpeed = value;
			foreach (CloudyCopterHelper helperInstance in CloudyCopterHelper.HelperInstances)
			{
				helperInstance.WindSpeed = value;
			}
		}
	}

	[DataMember]
	[Label<Strings>("EMITS_LIGHT_DURING_LIGHTS_OUT")]
	[Help<HelpText>("EMITS_LIGHT_DURING_LIGHTS_OUT_TITLE", "EMITS_LIGHT_DURING_LIGHTS_OUT_TEXT")]
	[Group<CharacterModifiersCloudyCopterLightsOutGroup>]
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
	[Group<CharacterModifiersCloudyCopterLightsOutGroup>]
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
	[Group<CharacterModifiersCloudyCopterLightsOutGroup>]
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
	[Label<Strings>("MIN_HALL_LENGTH")]
	[Help<HelpText>("MIN_HALL_LENGTH_TITLE", "MIN_HALL_LENGTH_TEXT")]
	[Group<CharacterModifiersCloudyCopterGeneralGroup>]
	public virtual int MinHallLength { get; set; } = 5;

	public CloudyCopterModifiers()
	{
		Helper.CreateCharacterGuiltMenu(Collection, typeof(CharacterModifiersCloudyCopterMenu));
		Helper.CreateCharacterInstanceMenu<Cumulo>(typeof(CharacterModifiersCloudyCopterMenu));
	}
}
