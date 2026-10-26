using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal record SideMenuMysteryRoomEventModifiers : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("MYSTERY_ROOM_EVENT");

	public Type ParentGroup => typeof(MainMenuGroupEventModifiers);
}
