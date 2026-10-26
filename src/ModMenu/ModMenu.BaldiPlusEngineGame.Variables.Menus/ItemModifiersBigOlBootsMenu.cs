using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class ItemModifiersBigOlBootsMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("BIG_OL_BOOTS");

	public Type ParentGroup => typeof(ItemModifiersMenuGroup);
}
