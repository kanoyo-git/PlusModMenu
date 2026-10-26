using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class ItemModifiersAppleMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("APPLE");

	public Type ParentGroup => typeof(ItemModifiersMenuGroup);
}
