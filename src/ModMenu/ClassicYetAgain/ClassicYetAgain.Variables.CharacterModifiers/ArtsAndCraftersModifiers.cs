using System.Runtime.Serialization;
using ClassicYetAgain.Interfaces;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Characters.Interfaces;
using PlusModMenu.Api.Interfaces;

namespace ClassicYetAgain.Variables.CharacterModifiers;

[DataContract]
public class ArtsAndCraftersModifiers : ILethalTouch, ICharacterModifiers, IModifiers, ICharacterGetsSpawnedInSpoopMode, IModifiersForCharacter<ArtsAndCrafters>
{
	[DataMember]
	[Label<Strings>("LETHAL_TOUCH")]
	[Help<HelpText>("LETHAL_TOUCH_TITLE", "LETHAL_TOUCH_TEXT")]
	[Group<CharacterModifiersCraftersGeneralGroup>]
	public virtual bool LethalTouch { get; set; }

	[DataMember]
	[Label<Strings>("OVERRIDE_JEALOUSY")]
	[Help<HelpText>("OVERRIDE_JEALOUSY_TITLE", "OVERRIDE_JEALOUSY_TEXT")]
	[Group<CharacterModifiersCraftersGeneralGroup>]
	public virtual bool OverrideJealousy { get; set; }

	[DataMember]
	[Label<Strings>("CUSTOM_JEALOUSY")]
	[Help<HelpText>("CUSTOM_JEALOUSY_TITLE", "CUSTOM_JEALOUSY_TEXT")]
	[Group<CharacterModifiersCraftersGeneralGroup>]
	public virtual int CustomJealousy { get; set; }

	[DataMember]
	[Label<Strings>("ACTIVATE_IN_SPOOP_MODE")]
	[Help<HelpText>("ACTIVATE_IN_SPOOP_MODE_TITLE", "ACTIVATE_IN_SPOOP_MODE_TEXT")]
	[Group<CharacterModifiersCraftersGeneralGroup>]
	public virtual bool ActivateInSpoopMode { get; set; } = true;
}
