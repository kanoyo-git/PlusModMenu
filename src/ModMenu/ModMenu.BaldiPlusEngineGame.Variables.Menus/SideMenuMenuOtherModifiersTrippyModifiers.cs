using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuMenuOtherModifiersTrippyModifiers : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("TRIPPY_MODIFIERS");

	public Type ParentGroup => typeof(MainMenuGroupOtherModifiers);
}
