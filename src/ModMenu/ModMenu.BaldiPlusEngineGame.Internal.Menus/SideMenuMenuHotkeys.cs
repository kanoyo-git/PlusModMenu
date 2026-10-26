using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Internal.Menus;

internal class SideMenuMenuHotkeys : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("HOTKEYS");

	public Type ParentGroup => typeof(MainMenuGroupModMenu);
}
