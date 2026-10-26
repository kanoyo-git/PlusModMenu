using System;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ClassicYetAgain.Variables.Menus;

internal record SideMenuMenuOtherModifiersSkybox : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("SKYBOX");

	public Type ParentGroup => typeof(MainMenuGroupOtherModifiers);
}
