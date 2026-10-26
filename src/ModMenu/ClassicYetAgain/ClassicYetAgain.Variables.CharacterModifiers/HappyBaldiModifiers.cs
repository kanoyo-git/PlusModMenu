using System;
using System.Linq;
using System.Runtime.Serialization;
using ClassicYetAgain.Variables.Menus;
using ModMenu;
using ModMenu.BaldiPlusEngineGame;
using ModMenu.BaldiPlusEngineGame.Patches;

using PlusModMenu.Api;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Characters;
using PlusModMenu.Api.Characters.Interfaces;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;

using UnityEngine;

namespace ClassicYetAgain.Variables.CharacterModifiers;

[DataContract]
public class HappyBaldiModifiers : AbstractCharacterModifiers<HappyBaldi>, ICharacterCanMove, ICharacterModifiers, IModifiers, ICharacterCanPlayAudio, ICharacterCanSee, ICharacterHasPhysicalAppearance, ICharacterHasPlayerTrigger
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
	[Group<SideMenuGroupHappyBaldiModifiersGeneralGeneral>]
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
	[Group<SideMenuGroupHappyBaldiModifiersGeneralGeneral>]
	public virtual float Speed { get; set; } = 1f;

	[DataMember]
	[Label<Strings>("SILENT")]
	[Help<HelpText>("SILENT_TITLE", "SILENT_TEXT")]
	[Group<SideMenuGroupHappyBaldiModifiersGeneralGeneral>]
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
				AudioManager[] componentsInChildren = item.GetComponentsInChildren<AudioManager>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].audioDevice.enabled = !value;
				}
			}
		}
	}

	[DataMember]
	[Label<Strings>("BLIND")]
	[Help<HelpText>("BLIND_TITLE", "BLIND_TEXT")]
	[Group<SideMenuGroupHappyBaldiModifiersGeneralGeneral>]
	public virtual bool IsBlind { get; set; }

	[DataMember]
	[Label<Strings>("SCALE")]
	[Help<HelpText>("SCALE_TITLE", "SCALE_TEXT")]
	[Group<SideMenuGroupHappyBaldiModifiersGeneralGeneral>]
	public virtual float Scale
	{
		get
		{
			return _scale;
		}
		set
		{
			_scale = value;
			foreach (CharacterCollectorItem item in Collection)
			{
				item.transform.localScale = Vector3.one * value;
			}
		}
	}

	[DataMember]
	[Label<Strings>("INVISIBLE")]
	[Help<HelpText>("INVISIBLE_TITLE", "INVISIBLE_TEXT")]
	[Group<SideMenuGroupHappyBaldiModifiersGeneralGeneral>]
	public virtual bool IsInvisible
	{
		get
		{
			return _isInvisible;
		}
		set
		{
			_isInvisible = value;
			foreach (CharacterCollectorItem item in Collection)
			{
				SpriteRenderer[] componentsInChildren = item.GetComponentsInChildren<SpriteRenderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].renderingLayerMask = ((!value) ? 1u : 0u);
				}
			}
		}
	}

	[DataMember]
	[Label<Strings>("IGNORES_PLAYER_TRIGGER")]
	[Help<HelpText>("IGNORES_PLAYER_TRIGGER_TITLE", "IGNORES_PLAYER_TRIGGER_TEXT")]
	[Group<SideMenuGroupHappyBaldiModifiersGeneralGeneral>]
	public virtual bool IgnoresPlayerTrigger { get; set; }

	[DataMember]
	[Label<Strings>("SLENDER_BALDI")]
	[Help<HelpText>("SLENDER_BALDI_TITLE", "SLENDER_BALDI_TEXT")]
	[Group<SideMenuGroupHappyBaldiModifiersGeneralGeneral>]
	public virtual bool SlenderBaldi { get; set; }

	[DataMember]
	[Label<Strings>("MOVES_WHILE_BEING_LOOKED_AT")]
	[Help<HelpText>("MOVES_WHILE_BEING_LOOKED_AT_TITLE", "MOVES_WHILE_BEING_LOOKED_AT_TEXT")]
	[Group<SideMenuGroupHappyBaldiModifiersGeneralGeneral>]
	public virtual bool MovesInSight { get; set; }

	[DataMember]
	[Label<ModMenu.Strings>("EMITS_LIGHT_DURING_LIGHTS_OUT")]
	[Help<ModMenu.HelpText>("EMITS_LIGHT_DURING_LIGHTS_OUT_TITLE", "EMITS_LIGHT_DURING_LIGHTS_OUT_TEXT")]
	[Group<SideMenu_Group_HappyBaldiModifiers_General_LightsOut>]
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
	[Label<ModMenu.Strings>("LIGHT_COLOR")]
	[Help<ModMenu.HelpText>("LIGHT_COLOR_TITLE", "LIGHT_COLOR_TEXT")]
	[Group<SideMenu_Group_HappyBaldiModifiers_General_LightsOut>]
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
	[Label<ModMenu.Strings>("LIGHT_STRENGTH")]
	[Help<ModMenu.HelpText>("LIGHT_STRENGTH_TITLE", "LIGHT_STRENGTH_TEXT")]
	[Group<SideMenu_Group_HappyBaldiModifiers_General_LightsOut>]
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

	public HappyBaldiModifiers()
	{
		Helper.CreateCharacterGuiltMenu(Collection, typeof(SideMenuMenuHappyBaldiModifiersGeneral));
		Helper.CreateCharacterInstanceMenu<HappyBaldi>(typeof(SideMenuMenuHappyBaldiModifiersGeneral), count =>
		{
			HappyBaldi.WithTemplate((HappyBaldi template) =>
			{
				AbstractCharacterModifiers<HappyBaldi>.Spawn(count, template);
			});
		});
	}
}
