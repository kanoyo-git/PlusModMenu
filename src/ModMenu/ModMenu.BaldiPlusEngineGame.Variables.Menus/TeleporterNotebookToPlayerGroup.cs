using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class TeleporterNotebookToPlayerGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("TELEPORT_NOTEBOOK_TO_PLAYER");

	public Type ParentMenu => typeof(TeleporterNotebooksMenu);
}
