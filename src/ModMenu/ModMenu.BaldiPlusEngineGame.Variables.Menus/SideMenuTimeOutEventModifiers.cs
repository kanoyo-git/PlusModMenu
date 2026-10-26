using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal record SideMenuTimeOutEventModifiers : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("TIME_OUT_EVENT");

	public Type ParentGroup => typeof(MainMenuGroupEventModifiers);
}
