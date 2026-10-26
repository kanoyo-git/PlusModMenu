using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class ItemModifiersNametagMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("FACULTY_NAMETAG");

	public Type ParentGroup => typeof(ItemModifiersMenuGroup);
}
