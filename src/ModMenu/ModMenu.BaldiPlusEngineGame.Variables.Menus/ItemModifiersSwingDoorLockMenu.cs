using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class ItemModifiersSwingDoorLockMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("SWINGING_DOOR_LOCK");

	public Type ParentGroup => typeof(ItemModifiersMenuGroup);
}
