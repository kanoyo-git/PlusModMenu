using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ClassicYetAgain.Variables.Menus;

internal record SideMenuMenuCameraModifiersGeneral : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("GENERAL");

	public Type ParentGroup => typeof(MainMenuGroupCameraModifiers);
}
