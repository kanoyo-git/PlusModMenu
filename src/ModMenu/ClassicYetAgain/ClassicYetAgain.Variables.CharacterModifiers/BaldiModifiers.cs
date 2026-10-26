using System.Runtime.Serialization;
using ClassicYetAgain.Interfaces;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Characters.Interfaces;
using PlusModMenu.Api.Interfaces;

namespace ClassicYetAgain.Variables.CharacterModifiers;

[DataContract]
public class BaldiModifiers : ICharacterGetsSpawnedInSpoopMode, ICharacterModifiers, IModifiers, IModifiersForCharacter<Baldi>
{
	[DataMember]
	[Label<Strings>("SLAP_SPEED_RANDOMIZER")]
	[Help<HelpText>("SLAP_SPEED_RANDOMIZER_TITLE", "SLAP_SPEED_RANDOMIZER_TEXT")]
	[Group<CharacterModifiersBaldiGeneralGroup>]
	public virtual bool SlapSpeedRandomizer { get; set; }

	[DataMember]
	[Label<Strings>("ACTIVATE_IN_SPOOP_MODE")]
	[Help<HelpText>("ACTIVATE_IN_SPOOP_MODE_TITLE", "ACTIVATE_IN_SPOOP_MODE_TEXT")]
	[Group<CharacterModifiersBaldiGeneralGroup>]
	public virtual bool ActivateInSpoopMode { get; set; } = true;
}
