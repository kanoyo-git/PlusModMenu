using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal record SideMenuFogEventModifiers : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("FOG_EVENT");

	public Type ParentGroup => typeof(MainMenuGroupEventModifiers);
}
