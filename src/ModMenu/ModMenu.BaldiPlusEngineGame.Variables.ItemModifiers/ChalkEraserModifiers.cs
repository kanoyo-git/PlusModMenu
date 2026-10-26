using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces;

namespace ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;

[DataContract]
internal class ChalkEraserModifiers : IModifiers
{
	[DataMember]
	[Label<Strings>("TIMER")]
	[Help<HelpText>("APPLE_TIMER_TITLE", "CHALK_ERASER_TIMER_TEXT")]
	[Group<ItemModifiersChalkEraserGeneralGroup>]
	public virtual float Timer { get; set; } = 60f;
}
