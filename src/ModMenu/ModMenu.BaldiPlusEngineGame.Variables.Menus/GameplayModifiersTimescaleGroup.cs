using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class GameplayModifiersTimescaleGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("TIMESCALE");

	public Type ParentMenu => typeof(GameplayModifiersGeneralMenu);
}
