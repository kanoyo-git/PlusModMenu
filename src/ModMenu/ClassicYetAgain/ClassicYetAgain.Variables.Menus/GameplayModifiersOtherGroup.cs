using System;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ClassicYetAgain.Variables.Menus;

internal class GameplayModifiersOtherGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("OTHER");

	public Type ParentMenu => typeof(GameplayModifiersGeneralMenu);
}
