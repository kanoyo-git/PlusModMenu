using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces;

namespace ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;

[DataContract]
internal class SwingDoorLockModifiers : IModifiers
{
	[DataMember]
	[Label<Strings>("LOCK_TIME")]
	[Help<HelpText>("SWING_DOOR_LOCK_TIME_TITLE", "SWING_DOOR_LOCK_TIME_TEXT")]
	[Group<ItemModifiersSwingDoorLockGeneralGroup>]
	public virtual float LockTime { get; set; } = 30f;
}
