using System.Runtime.Serialization;
using ClassicYetAgain.Interfaces;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Characters.Interfaces;
using PlusModMenu.Api.Interfaces;

namespace ClassicYetAgain.Variables.CharacterModifiers;

[DataContract]
public class PlaytimeModifiers : ICharacterGetsSpawnedInSpoopMode, ICharacterModifiers, IModifiers, ILethalTouch, IModifiersForCharacter<Playtime>
{
	[DataMember]
	[Label<Strings>("AGGRESSIVE_PLAYTIME")]
	[Help<HelpText>("AGGRESSIVE_PLAYTIME_TITLE", "AGGRESSIVE_PLAYTIME_TEXT")]
	[Group<CharacterModifiersPlaytimeGeneralGroup>]
	public virtual bool Aggressive { get; set; }

	[DataMember]
	[Label<Strings>("LETHAL_TOUCH")]
	[Help<HelpText>("LETHAL_TOUCH_TITLE", "LETHAL_TOUCH_TEXT")]
	[Group<CharacterModifiersPlaytimeGeneralGroup>]
	public virtual bool LethalTouch { get; set; }

	[DataMember]
	[Label<Strings>("PLAYTIME_DELIVERY_SERVICE")]
	[Help<HelpText>("PLAYTIME_DELIVERY_SERVICE_TITLE", "PLAYTIME_DELIVERY_SERVICE_TEXT")]
	[Group<CharacterModifiersPlaytimeGeneralGroup>]
	public virtual bool DeliveryService { get; set; }

	[DataMember]
	[Label<Strings>("ACTIVATE_IN_SPOOP_MODE")]
	[Help<HelpText>("ACTIVATE_IN_SPOOP_MODE_TITLE", "ACTIVATE_IN_SPOOP_MODE_TEXT")]
	[Group<CharacterModifiersPlaytimeGeneralGroup>]
	public virtual bool ActivateInSpoopMode { get; set; } = true;
}
