using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class TeleporterPlayerToNotebookGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("TELEPORT_PLAYER_TO_NOTEBOOK");

	public Type ParentMenu => typeof(TeleporterNotebooksMenu);
}
