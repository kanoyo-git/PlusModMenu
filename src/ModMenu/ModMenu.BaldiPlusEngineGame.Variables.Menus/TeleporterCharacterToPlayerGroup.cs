using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class TeleporterCharacterToPlayerGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("TELEPORT_CHARACTER_TO_PLAYER");

	public Type ParentMenu => typeof(TeleporterCharactersMenu);
}
