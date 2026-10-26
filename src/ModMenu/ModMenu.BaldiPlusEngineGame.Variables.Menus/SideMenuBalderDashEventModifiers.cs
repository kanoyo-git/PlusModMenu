using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal record SideMenuBalderDashEventModifiers : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("BALDER_DASH_EVENT");

	public Type ParentGroup => typeof(MainMenuGroupEventModifiers);
}
