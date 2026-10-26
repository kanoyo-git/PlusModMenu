using System;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ClassicYetAgain.Variables.Menus;

internal class SideMenuMenuHappyBaldiModifiersGeneral : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("HAPPY_BALDI");

	public Type ParentGroup => typeof(CharacterModifiersMenuGroup);
}
