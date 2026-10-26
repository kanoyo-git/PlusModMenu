using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal record SideMenuLockdownEventModifiers : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("LOCKDOWN_EVENT");

	public Type ParentGroup => typeof(MainMenuGroupEventModifiers);
}
