using System.Runtime.Serialization;
using PlusModMenu.Core.Unity.Extensions;
using ModMenu.BaldiPlusEngineGame.Patches;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces;
using TMPro;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables.OtherModifiers;

internal class TrippyModifiers : IModifiers
{
	private static readonly VideoDiarrheaManager _videoDiarrheaManager = new VideoDiarrheaManager();

	private static readonly DiscoManager _discoManager = new DiscoManager();

	private bool _scrabbleEnabled;

	[DataMember]
	[Label<Strings>("VIDEO_DIARRHEA")]
	[Help<HelpText>("VIDEO_DIARRHEA_TITLE", "VIDEO_DIARRHEA_TEXT")]
	[Group<SideMenuGroupOtherModifiersVideoDiarrhea>]
	public virtual bool VideoDiarrheaEnabled
	{
		get
		{
			return _videoDiarrheaManager.Enabled;
		}
		set
		{
			_videoDiarrheaManager.Enabled = value;
		}
	}

	[DataMember]
	[Label<Strings>("MIN_COOLDOWN")]
	[Help<HelpText>("VIDEO_DIARRHEA_MIN_COOLDOWN_TITLE", "VIDEO_DIARRHEA_MIN_COOLDOWN_TEXT")]
	[Group<SideMenuGroupOtherModifiersVideoDiarrhea>]
	public virtual float VideoDiarrheaMinCooldown
	{
		get
		{
			return _videoDiarrheaManager.MinCooldown;
		}
		set
		{
			_videoDiarrheaManager.MinCooldown = value;
		}
	}

	[DataMember]
	[Label<Strings>("MIN_COOLDOWN")]
	[Help<HelpText>("VIDEO_DIARRHEA_MAX_COOLDOWN_TITLE", "VIDEO_DIARRHEA_MAX_COOLDOWN_TEXT")]
	[Group<SideMenuGroupOtherModifiersVideoDiarrhea>]
	public virtual float VideoDiarrheaMaxCooldown
	{
		get
		{
			return _videoDiarrheaManager.MaxCooldown;
		}
		set
		{
			_videoDiarrheaManager.MaxCooldown = value;
		}
	}

	[DataMember]
	[Label<Strings>("AUDIO_VOMIT")]
	[Help<HelpText>("AUDIO_VOMIT_TITLE", "AUDIO_VOMIT_TEXT")]
	[Group<SideMenuGroupOtherModifiersAudioVomit>]
	public virtual bool AudioVomitEnabled { get; set; }

	[DataMember]
	[Label<Strings>("SCRABBLE")]
	[Help<HelpText>("SCRABBLE_TITLE", "SCRABBLE_TEXT")]
	[Group<SideMenuGroupOtherModifiersScrabble>]
	public virtual bool ScrabbleEnabled
	{
		get
		{
			return _scrabbleEnabled;
		}
		set
		{
			_scrabbleEnabled = value;
			if (!value)
			{
				return;
			}
			TMP_Text[] array = Resources.FindObjectsOfTypeAll<TMP_Text>();
			foreach (TMP_Text tMP_Text in array)
			{
				if (!(tMP_Text.gameObject == null) && tMP_Text.gameObject.scene.IsGameScene())
				{
					tMP_Text.text = tMP_Text.text;
				}
			}
		}
	}

	[DataMember]
	[Label<Strings>("DISCO")]
	[Help<HelpText>("DISCO_TITLE", "DISCO_TEXT")]
	[Group<SideMenuGroupOtherModifiersDisco>]
	public virtual bool DiscoEnabled
	{
		get
		{
			return _discoManager.Enabled;
		}
		set
		{
			_discoManager.Enabled = value;
		}
	}

	[DataMember]
	[Label<Strings>("MIN_COOLDOWN")]
	[Help<HelpText>("VIDEO_DIARRHEA_MIN_COOLDOWN_TITLE", "DISCO_MIN_COOLDOWN_TEXT")]
	[Group<SideMenuGroupOtherModifiersDisco>]
	public virtual float DiscoMinCooldown
	{
		get
		{
			return _discoManager.MinCooldown;
		}
		set
		{
			_discoManager.MinCooldown = value;
		}
	}

	[DataMember]
	[Label<Strings>("MAX_COOLDOWN")]
	[Help<HelpText>("VIDEO_DIARRHEA_MAX_COOLDOWN_TITLE", "DISCO_MAX_COOLDOWN_TEXT")]
	[Group<SideMenuGroupOtherModifiersDisco>]
	public virtual float DiscoMaxCooldown
	{
		get
		{
			return _discoManager.MaxCooldown;
		}
		set
		{
			_discoManager.MaxCooldown = value;
		}
	}

	[DataMember]
	[Label<Strings>("VECTOR3_CORRUPTION")]
	[Help<HelpText>("VECTOR3_CORRUPTION_TITLE", "VECTOR3_CORRUPTION_TEXT")]
	[Group<SideMenuGroupOtherModifiersVector3Corruption>]
	public virtual bool Vector3CorruptionEnabled
	{
		get
		{
			return Vector3Patch.CorruptionActive;
		}
		set
		{
			Vector3Patch.CorruptionActive = value;
		}
	}

	[DataMember]
	[Label<Strings>("CORRUPTION_STRENGTH")]
	[Help<HelpText>("CORRUPTION_STRENGTH_TITLE", "CORRUPTION_STRENGTH_TEXT")]
	[Group<SideMenuGroupOtherModifiersVector3Corruption>]
	public virtual float Vector3CorruptionStrength
	{
		get
		{
			return Vector3Patch.Strength;
		}
		set
		{
			Vector3Patch.Strength = value;
		}
	}

	[DataMember]
	[Label<Strings>("VECTOR2_CORRUPTION")]
	[Help<HelpText>("VECTOR2_CORRUPTION_TITLE", "VECTOR2_CORRUPTION_TEXT")]
	[Group<SideMenuGroupOtherModifiersVector2Corruption>]
	public virtual bool Vector2CorruptionEnabled
	{
		get
		{
			return Vector2Patch.CorruptionActive;
		}
		set
		{
			Vector2Patch.CorruptionActive = value;
		}
	}

	[DataMember]
	[Label<Strings>("CORRUPTION_STRENGTH")]
	[Help<HelpText>("CORRUPTION_STRENGTH_TITLE", "CORRUPTION_STRENGTH_TEXT")]
	[Group<SideMenuGroupOtherModifiersVector2Corruption>]
	public virtual float Vector2CorruptionStrength
	{
		get
		{
			return Vector2Patch.Strength;
		}
		set
		{
			Vector2Patch.Strength = value;
		}
	}
}
