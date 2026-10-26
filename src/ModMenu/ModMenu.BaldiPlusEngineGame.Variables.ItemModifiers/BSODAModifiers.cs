using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces;

namespace ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;

[DataContract]
internal class BSODAModifiers : IModifiers
{
	[DataMember]
	[Label<Strings>("VELOCITY")]
	[Help<HelpText>("BSODA_VELOCITY_TITLE", "BSODA_VELOCITY_TEXT")]
	[Group<ItemModifiersBsodaGeneralGroup>]
	public virtual float Velocity { get; set; } = 30f;

	[DataMember]
	[Label<Strings>("LIFE_SPAN")]
	[Help<HelpText>("BSODA_LIFE_SPAN_TITLE", "BSODA_LIFE_SPAN_TEXT")]
	[Group<ItemModifiersBsodaGeneralGroup>]
	public virtual float LifeSpan { get; set; } = 30f;
}
