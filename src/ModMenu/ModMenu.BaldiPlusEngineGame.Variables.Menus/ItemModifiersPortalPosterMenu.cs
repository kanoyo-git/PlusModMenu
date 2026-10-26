using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class ItemModifiersPortalPosterMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("PORTAL_POSTER");

	public Type ParentGroup => typeof(ItemModifiersMenuGroup);
}
