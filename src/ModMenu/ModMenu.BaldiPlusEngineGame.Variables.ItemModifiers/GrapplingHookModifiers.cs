using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces;

namespace ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;

[DataContract]
internal class GrapplingHookModifiers : IModifiers
{
	[DataMember]
	[Label<Strings>("VELOCITY_INCREASE")]
	[Help<HelpText>("VELOCITY_INCREASE_TITLE", "VELOCITY_INCREASE_TITLE")]
	[Group<ItemModifiersGrapplingHookGeneralGroup>]
	public virtual float VelocityIncrease { get; set; } = 20f;

	[DataMember]
	[Label<Strings>("VELOCITY_START")]
	[Help<HelpText>("VELOCITY_START_TITLE", "VELOCITY_START_TEXT")]
	[Group<ItemModifiersGrapplingHookGeneralGroup>]
	public virtual float VelocityStart { get; set; } = 50f;

	[DataMember]
	[Label<Strings>("MAX_PRESSURE")]
	[Help<HelpText>("MAX_PRESSURE_TITLE", "MAX_PRESSURE_TEXT")]
	[Group<ItemModifiersGrapplingHookGeneralGroup>]
	public virtual float MaxPressure { get; set; } = 100f;
}
