using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces;

namespace ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;

[DataContract]
internal class BigOlBootsModifiers : IModifiers
{
	[DataMember]
	[Label<Strings>("TIMER")]
	[Help<HelpText>("APPLE_TIMER_TITLE", "BIG_OL_BOOTS_TIMER_TEXT")]
	[Group<ItemModifiersBigOlBootsGeneralGroup>]
	public virtual float Timer { get; set; } = 15f;
}
