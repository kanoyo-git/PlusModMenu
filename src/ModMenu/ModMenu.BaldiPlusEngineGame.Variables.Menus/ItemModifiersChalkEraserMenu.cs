using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class ItemModifiersChalkEraserMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("DIRTY_CHALK_ERASER");

	public Type ParentGroup => typeof(ItemModifiersMenuGroup);
}
