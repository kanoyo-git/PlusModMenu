using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuMenuOtherModifiersFunSettings : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("FUN_SETTINGS");

	public Type ParentGroup => typeof(MainMenuGroupOtherModifiers);
}
