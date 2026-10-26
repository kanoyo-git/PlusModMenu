using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces;

namespace ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;

[DataContract]
internal class WDNoSqueeModifiers : IModifiers
{
	[DataMember]
	[Label<Strings>("TIMER")]
	[Help<HelpText>("APPLE_TIMER_TITLE", "WD_NOSQUEE_TIMER_TEXT")]
	[Group<ItemModifiersWdNoSqueeGeneralGroup>]
	public virtual float Timer { get; set; } = 300f;

	[DataMember]
	[Label<Strings>("SILENCE_RADIUS")]
	[Help<HelpText>("WD_NOSQUEE_SILENCE_RADIUS_TITLE", "WD_NOSQUEE_SILENCE_RADIUS_TEXT")]
	[Group<ItemModifiersWdNoSqueeGeneralGroup>]
	[ValueRange(0, int.MaxValue)]
	public virtual int Radius { get; set; } = 4;
}
