using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces;

namespace ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;

[DataContract]
internal class ZestyBarModifiers : IModifiers
{
	[DataMember]
	[Label<Strings>("STAMINA_MULTIPLIER")]
	[Help<HelpText>("ZESTY_BAR_STAMINA_MULTIPLIER_TITLE", "ZESTY_BAR_STAMINA_MULTIPLIER_TEXT")]
	[Group<ItemModifiersZestyBarGeneralGroup>]
	public virtual float StaminaMultiplier { get; set; } = 2f;
}
