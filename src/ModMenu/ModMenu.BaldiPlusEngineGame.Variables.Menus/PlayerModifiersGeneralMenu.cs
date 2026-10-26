using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class PlayerModifiersGeneralMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("GENERAL");

	public Type ParentGroup => typeof(PlayerModifiersMenuGroup);
}
