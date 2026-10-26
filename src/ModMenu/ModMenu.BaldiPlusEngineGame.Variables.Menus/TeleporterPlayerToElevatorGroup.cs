using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class TeleporterPlayerToElevatorGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("TELEPORT_PLAYER_TO_EXIT");

	public Type ParentMenu => typeof(TeleporterElevatorMenu);
}
