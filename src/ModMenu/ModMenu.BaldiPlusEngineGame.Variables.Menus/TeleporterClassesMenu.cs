using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class TeleporterClassesMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("CLASSES");

	public Type ParentGroup => typeof(TeleporterMenuGroup);
}
