using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class TeleporterPlayerToCharacterGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("TELEPORT_PLAYER_TO_CHARACTER");

	public Type ParentMenu => typeof(TeleporterCharactersMenu);
}
