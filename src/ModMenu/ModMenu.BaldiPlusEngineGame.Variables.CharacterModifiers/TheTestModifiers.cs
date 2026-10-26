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
public class TheTestModifiers : AbstractCharacterModifiers<LookAtGuy>, ICharacterCanMove, ICharacterModifiers, IModifiers, ICharacterCanPlayAudio, ICharacterHasPhysicalAppearance, ICharacterHasPlayerTrigger, ICharacterLanternModeModifiers, ILanternModeSourceModifiers
{
	private bool _isSilent;

	private float _scale = 1f;

	private bool _isInvisible;

	private bool _emitsLightDuringLightsOut;

	private Color _lightsOutLightColor = Color.white;

	private float _lightsOutStrength = 4f;

	private static readonly AccessTools.FieldRef<LookAtGuy, float> _fogTimeField = AccessTools.FieldRefAccess<LookAtGuy, float>("fogTime");

	private static readonly AccessTools.FieldRef<LookAtGuy, float> _explodeChanceField = AccessTools.FieldRefAccess<LookAtGuy, float>("explodeChance");

	private static readonly AccessTools.FieldRef<LookAtGuy, float> _fleeSpeedField = AccessTools.FieldRefAccess<LookAtGuy, float>("fleeSpeed");

	private float _fogTime = 120f;

	private float _explodeChance = 0.1f;

	private float _fleeSpeed = 100f;

	[DataMember]
	[Label<Strings>("ENABLED")]
	[Help<HelpText>("ENABLED_TITLE", "ENABLED_TEXT")]
	[Group<CharacterModifiersTheTestGeneralGroup>]
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
	[Group<CharacterModifiersTheTestGeneralGroup>]
	public virtual float Speed { get; set; } = 1f;

	[DataMember]
	[Label<Strings>("SILENT")]
	[Help<HelpText>("SILENT_TITLE", "SILENT_TEXT")]
	[Group<CharacterModifiersTheTestGeneralGroup>]
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
	[Group<CharacterModifiersTheTestGeneralGroup>]
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
	[Group<CharacterModifiersTheTestGeneralGroup>]
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
	[Group<CharacterModifiersTheTestGeneralGroup>]
	public virtual bool IgnoresPlayerTrigger { get; set; }

	[DataMember]
	[Label<Strings>("THE_TEST_FOG_TIME")]
	[Help<HelpText>("THE_TEST_FOG_TIME_TITLE", "THE_TEST_FOG_TIME_TEXT")]
	[Group<CharacterModifiersTheTestGeneralGroup>]
	public virtual float FogTime
	{
		get
		{
			return _fogTime;
		}
		set
		{
			_fogTime = value;
			ApplyAll();
		}
	}

	[DataMember]
	[Label<Strings>("THE_TEST_EXPLODE_CHANCE")]
	[Help<HelpText>("THE_TEST_EXPLODE_CHANCE_TITLE", "THE_TEST_EXPLODE_CHANCE_TEXT")]
	[Group<CharacterModifiersTheTestGeneralGroup>]
	public virtual float ExplodeChance
	{
		get
		{
			return _explodeChance;
		}
		set
		{
			_explodeChance = value;
			ApplyAll();
		}
	}

	[DataMember]
	[Label<Strings>("THE_TEST_FLEE_SPEED")]
	[Help<HelpText>("THE_TEST_FLEE_SPEED_TITLE", "THE_TEST_FLEE_SPEED_TEXT")]
	[Group<CharacterModifiersTheTestGeneralGroup>]
	public virtual float FleeSpeed
	{
		get
		{
			return _fleeSpeed;
		}
		set
		{
			_fleeSpeed = value;
			ApplyAll();
		}
	}

	[DataMember]
	[Label<Strings>("EMITS_LIGHT_DURING_LIGHTS_OUT")]
	[Help<HelpText>("EMITS_LIGHT_DURING_LIGHTS_OUT_TITLE", "EMITS_LIGHT_DURING_LIGHTS_OUT_TEXT")]
	[Group<CharacterModifiersTheTestLightsOutGroup>]
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
	[Group<CharacterModifiersTheTestLightsOutGroup>]
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
	[Group<CharacterModifiersTheTestLightsOutGroup>]
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

	public TheTestModifiers()
	{
		Helper.CreateCharacterGuiltMenu(Collection, typeof(CharacterModifiersTheTestMenu));
		Helper.CreateCharacterInstanceMenu<LookAtGuy>(typeof(CharacterModifiersTheTestMenu));
	}

	internal void Apply(LookAtGuy test)
	{
		_fogTimeField(test) = _fogTime;
		_explodeChanceField(test) = _explodeChance;
		_fleeSpeedField(test) = _fleeSpeed;
	}

	private void ApplyAll()
	{
		foreach (var item in Collection)
		{
			Apply((LookAtGuy)item.Item);
		}
	}

	[HarmonyPatch(typeof(LookAtGuy), nameof(LookAtGuy.Initialize))]
	private class InitializePatch
	{
		private static void Postfix(LookAtGuy __instance)
		{
			PlusModMenu.Api.Service.Get<PlusModMenu.Api.Interfaces.Services.IModMenuService>().Get<TheTestModifiers>().Apply(__instance);
		}
	}
}
