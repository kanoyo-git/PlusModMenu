using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuMenuOtherModifiersStickers : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("STICKERS");

	public Type ParentGroup => typeof(MainMenuGroupOtherModifiers);
}
