using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class TeleporterElevatorMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("EXITS");

	public Type ParentGroup => typeof(TeleporterMenuGroup);
}
