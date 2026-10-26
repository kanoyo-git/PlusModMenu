using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuMenuOtherModifiersPlayerData : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("PLAYER_DATA");

	public Type ParentGroup => typeof(MainMenuGroupOtherModifiers);
}
