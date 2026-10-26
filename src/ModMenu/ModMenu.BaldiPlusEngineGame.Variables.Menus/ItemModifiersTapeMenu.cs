using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class ItemModifiersTapeMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("BALDIS_LEAST_FAVORITE_TAPE");

	public Type ParentGroup => typeof(ItemModifiersMenuGroup);
}
