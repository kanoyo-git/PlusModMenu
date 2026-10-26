using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class ItemModifiersAlarmClockMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("ALARM_CLOCK");

	public Type ParentGroup => typeof(ItemModifiersMenuGroup);
}
