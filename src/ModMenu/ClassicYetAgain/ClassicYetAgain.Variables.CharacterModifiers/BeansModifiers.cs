using System.Runtime.Serialization;
using ClassicYetAgain.Interfaces;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Characters.Interfaces;
using PlusModMenu.Api.Interfaces;

namespace ClassicYetAgain.Variables.CharacterModifiers;

[DataContract]
public class BeansModifiers : ICharacterGetsSpawnedInSpoopMode, ICharacterModifiers, IModifiers, ILethalTouch, IModifiersForCharacter<Beans>
{
	[DataMember]
	[Label<Strings>("LETHAL_TOUCH")]
	[Help<HelpText>("LETHAL_TOUCH_TITLE", "LETHAL_TOUCH_TEXT")]
	[Group<CharacterModifiersBeansGeneralGroup>]
	public virtual bool LethalTouch { get; set; }

	[DataMember]
	[Label<Strings>("LETHAL_TOUCH_ON_GUM_WAD")]
	[Help<HelpText>("LETHAL_TOUCH_ON_GUM_WAD_TITLE", "LETHAL_TOUCH_ON_GUM_WAD_TEXT")]
	[Group<CharacterModifiersBeansGeneralGroup>]
	public virtual bool LethalTouchOnGumWad { get; set; }

	[DataMember]
	[Label<Strings>("ACTIVATE_IN_SPOOP_MODE")]
	[Help<HelpText>("ACTIVATE_IN_SPOOP_MODE_TITLE", "ACTIVATE_IN_SPOOP_MODE_TEXT")]
	[Group<CharacterModifiersBeansGeneralGroup>]
	public virtual bool ActivateInSpoopMode { get; set; } = true;
}
