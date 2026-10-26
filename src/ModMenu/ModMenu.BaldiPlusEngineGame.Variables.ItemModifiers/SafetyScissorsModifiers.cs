using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces;

namespace ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;

[DataContract]
internal class SafetyScissorsModifiers : IModifiers
{
	[DataMember]
	[Label<Strings>("FIRST_PRIZE_CRAZY_TIME")]
	[Help<HelpText>("FIRST_PRIZE_CRAZY_TIME_TITLE", "FIRST_PRIZE_CRAZY_TIME_TEXT")]
	[Group<ItemModifiersSafetyScissorsGeneralGroup>]
	public virtual float FirstPrizeCrazyTime { get; set; } = 30f;
}
