using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class ItemModifiersZestyBarMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("ENERGY_FLAVORED_ZESTY_BAR");

	public Type ParentGroup => typeof(ItemModifiersMenuGroup);
}
