using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuMenuOtherModifiersWorld : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("WORLD");

	public Type ParentGroup => typeof(MainMenuGroupOtherModifiers);
}
