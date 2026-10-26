using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using PlusModMenu.Core.Common;
using PlusModMenu.Core.Unity.Extensions;
using ModMenu.BaldiPlusEngineGame.Interfaces;
using ModMenu.BaldiPlusEngineGame.Patches;
using ModMenu.BaldiPlusEngineGame.Settings;
using ModMenu.BaldiPlusEngineGame.Utils;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using ModMenu.Common;
using PlusModMenu.Api;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Characters;
using PlusModMenu.Api.Characters.Interfaces;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using PlusModMenu.Api.Settings;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;

[DataContract]
public class PrincipalModifiers : AbstractCharacterModifiers<Principal>, ICharacterCanHear, ICharacterModifiers, IModifiers, ICharacterCanMove, ICharacterCanPlayAudio, ICharacterCanSee, ICharacterHasPhysicalAppearance, ICharacterHasPlayerTrigger, ICharacterLanternModeModifiers, ILanternModeSourceModifiers
{
	private bool _isSilent;

	private float _scale = 1f;

	private bool _isInvisible;

	private bool _emitsLightDuringLightsOut = true;

	private Color _lightsOutLightColor = Color.white;

	private float _lightsOutStrength = 4f;

	public static List<ISettingHandle> RuleChoices { get; } = new List<ISettingHandle>();

	public static List<string> RuleIds
	{
		get
		{
			PrincipalModifiers? principal = Service.Get<IModMenuService>().Get<PrincipalModifiers>();
			return principal == null ? new List<string>() : principal.Rules.Select(x => x.Key).ToList();
		}
	}

	[DataMember]
	[Label<Strings>("ENABLED")]
	[Help<HelpText>("ENABLED_TITLE", "ENABLED_TEXT")]
	[Group<CharacterModifiersPrincipalGeneralGroup>]
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
	[Label<Strings>("DEAF")]
	[Help<HelpText>("DEAF_TITLE", "DEAF_TEXT")]
	[Group<CharacterModifiersPrincipalGeneralGroup>]
	public virtual bool IsDeaf { get; set; }

	[DataMember]
	[Label<Strings>("SPEED")]
	[Help<HelpText>("SPEED_TITLE", "SPEED_TEXT")]
	[Group<CharacterModifiersPrincipalGeneralGroup>]
	public virtual float Speed { get; set; } = 1f;

	[DataMember]
	[Label<Strings>("SILENT")]
	[Help<HelpText>("SILENT_TITLE", "SILENT_TEXT")]
	[Group<CharacterModifiersPrincipalGeneralGroup>]
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
	[Group<CharacterModifiersPrincipalGeneralGroup>]
	public virtual bool IsBlind { get; set; }

	[DataMember]
	[Label<Strings>("SCALE")]
	[Help<HelpText>("SCALE_TITLE", "SCALE_TEXT")]
	[Group<CharacterModifiersPrincipalGeneralGroup>]
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
	[Group<CharacterModifiersPrincipalGeneralGroup>]
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
	[Group<CharacterModifiersPrincipalGeneralGroup>]
	public virtual bool IgnoresPlayerTrigger { get; set; }

	[DataMember]
	[Label<Strings>("DETENTION_TIME_INCREASE")]
	[Help<HelpText>("DETENTION_TIME_INCREASE_TITLE", "DETENTION_TIME_INCREASE_TEXT")]
	[Group<CharacterModifiersPrincipalGeneralGroup>]
	public virtual int DetentionTimeIncrease { get; set; } = 5;

	[DataMember]
	public CallbackDictionary<string, bool> Rules { get; set; } = new CallbackDictionary<string, bool>
	{
		{ "Running", true },
		{ "Faculty", true },
		{ "Drinking", true },
		{ "Escaping", true },
		{ "Lockers", true },
		{ "AfterHours", true },
		{ "Bullying", true }
	};

	[DataMember]
	[Label<Strings>("EMITS_LIGHT_DURING_LIGHTS_OUT")]
	[Help<HelpText>("EMITS_LIGHT_DURING_LIGHTS_OUT_TITLE", "EMITS_LIGHT_DURING_LIGHTS_OUT_TEXT")]
	[Group<CharacterModifiersPrincipalLightsOutGroup>]
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
	[Group<CharacterModifiersPrincipalLightsOutGroup>]
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
	[Group<CharacterModifiersPrincipalLightsOutGroup>]
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

	public PrincipalModifiers()
	{
		PrincipalModifiers principalModifiers = this;
		Helper.CreateCharacterGuiltMenu(Collection, typeof(CharacterModifiersPrincipalMenu));
		Helper.CreateCharacterInstanceMenu<Principal>(typeof(CharacterModifiersPrincipalMenu));
		Service.Get<IEventService>().SubscribeOnce<IModMenuInitializedEvent>((object? _) =>
		{
			ISettingsRegistry registry = SettingsRegistryService.Registry;
			ISettingsGroup rulesGroup = registry.GetGroup(typeof(CharacterModifiersPrincipalRulesGroup));
			rulesGroup.AddButton(UIText.Resource<Strings>("ADD_NEW_RULE"), () =>
			{
				registry.Dialogs.Prompt(UIText.Resource<Strings>("ADD_NEW_RULE"), UIText.Resource<Strings>("RULE_ID"), string.Empty, UIText.Resource<Strings>("ADD"), value =>
				{
					if (!string.IsNullOrWhiteSpace(value) && !principalModifiers.Rules.ContainsKey(value))
					{
						principalModifiers.Rules.Add(value, value: true);
					}
				});
			}, UIText.Resource<HelpText>("ADD_NEW_RULE_TEXT"));
			principalModifiers.Rules.ItemAdded += (string key, bool flag) =>
			{
				Debugger.Log("Added " + key);
				AddRuleToggle(key);
			};
			foreach (KeyValuePair<string, bool> rule in principalModifiers.Rules)
			{
				AddRuleToggle(rule.Key);
			}
			void AddRuleToggle(string ruleId)
			{
				rulesGroup.AddToggle(UIText.Literal(ruleId), () => principalModifiers.Rules.TryGetValue(ruleId, out bool value) && value, x =>
				{
					principalModifiers.Rules[ruleId] = x;
				});
				foreach (ISettingHandle choice in RuleChoices)
				{
					choice.Refresh();
				}
			}
		});
	}
}
