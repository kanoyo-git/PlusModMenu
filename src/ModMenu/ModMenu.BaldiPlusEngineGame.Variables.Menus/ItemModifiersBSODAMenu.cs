using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class ItemModifiersBSODAMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("BSODA");

	public Type ParentGroup => typeof(ItemModifiersMenuGroup);
}
