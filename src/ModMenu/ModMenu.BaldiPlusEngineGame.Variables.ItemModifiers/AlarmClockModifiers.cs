using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces;

namespace ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;

[DataContract]
internal class AlarmClockModifiers : IModifiers
{
	[DataMember]
	[Label<Strings>("ALARM_TIMER_MULTIPLIER")]
	[Help<HelpText>("ALARM_TIMER_MULTIPLIER_TITLE", "ALARM_TIMER_MULTIPLIER_TEXT")]
	[Group<ItemModifiersAlarmClockGeneralGroup>]
	public virtual float AlarmTimerMultiplier { get; set; } = 1f;
}
