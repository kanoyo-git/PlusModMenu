using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal record SideMenuRulerEventModifiers : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("RULER_EVENT");

	public Type ParentGroup => typeof(MainMenuGroupEventModifiers);
}
