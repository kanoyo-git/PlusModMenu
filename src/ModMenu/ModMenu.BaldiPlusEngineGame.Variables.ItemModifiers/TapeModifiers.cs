using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces;

namespace ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;

[DataContract]
internal class TapeModifiers : IModifiers
{
	[DataMember]
	[Label<Strings>("ANTI_HEARING_TIME")]
	[Help<HelpText>("TAPE_ANTI_HEARING_TIME_TITLE", "TAPE_ANTI_HEARING_TIME_TEXT")]
	[Group<ItemModifiersTapeGeneralGroup>]
	public virtual float AntiHearingTime { get; set; } = 30f;
}
