using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class ItemModifiersWDNoSqueeMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("WD_NO_SQUEE");

	public Type ParentGroup => typeof(ItemModifiersMenuGroup);
}
