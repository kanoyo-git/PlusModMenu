using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces;

namespace ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;

[DataContract]
internal class AppleModifiers : IModifiers
{
	[DataMember]
	[Label<Strings>("TIMER")]
	[Help<HelpText>("APPLE_TIMER_TITLE", "APPLE_TIMER_TEXT")]
	[Group<ItemModifiersAppleGeneralGroup>]
	public virtual float Timer { get; set; } = 15f;
}
