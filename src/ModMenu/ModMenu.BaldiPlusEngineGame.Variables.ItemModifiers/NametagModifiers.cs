using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces;

namespace ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;

[DataContract]
internal class NametagModifiers : IModifiers
{
	[DataMember]
	[Label<Strings>("TIMER")]
	[Help<HelpText>("APPLE_TIMER_TITLE", "NAMETAG_TIMER_TEXT")]
	[Group<ItemModifiersNametagGeneralGroup>]
	public virtual float Timer { get; set; } = 30f;
}
