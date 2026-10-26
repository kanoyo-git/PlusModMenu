using System.Runtime.Serialization;
using ClassicYetAgain.Interfaces;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Characters.Interfaces;
using PlusModMenu.Api.Interfaces;

namespace ClassicYetAgain.Variables.CharacterModifiers;

[DataContract]
public class CumuloModifiers : ICharacterGetsSpawnedInSpoopMode, ICharacterModifiers, IModifiers, ILethalTouch, IModifiersForCharacter<Cumulo>
{
	[DataMember]
	[Label<Strings>("LETHAL_TOUCH")]
	[Help<HelpText>("LETHAL_TOUCH_TITLE", "LETHAL_TOUCH_TEXT")]
	[Group<CharacterModifiersCloudyCopterGeneralGroup>]
	public virtual bool LethalTouch { get; set; }

	[DataMember]
	[Label<Strings>("ACTIVATE_IN_SPOOP_MODE")]
	[Help<HelpText>("ACTIVATE_IN_SPOOP_MODE_TITLE", "ACTIVATE_IN_SPOOP_MODE_TEXT")]
	[Group<CharacterModifiersCloudyCopterGeneralGroup>]
	public virtual bool ActivateInSpoopMode { get; set; } = true;
}
