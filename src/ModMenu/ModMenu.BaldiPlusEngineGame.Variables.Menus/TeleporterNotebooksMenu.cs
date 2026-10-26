using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class TeleporterNotebooksMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("NOTEBOOKS");

	public Type ParentGroup => typeof(TeleporterMenuGroup);
}
