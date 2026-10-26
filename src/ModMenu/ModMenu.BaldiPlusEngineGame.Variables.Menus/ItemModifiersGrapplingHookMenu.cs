using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class ItemModifiersGrapplingHookMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("GRAPPLING_HOOK");

	public Type ParentGroup => typeof(ItemModifiersMenuGroup);
}
