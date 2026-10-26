using System;
using ModMenu;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ClassicYetAgain.Variables.Menus;

internal class SideMenu_Group_HappyBaldiModifiers_General_LightsOut : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<ModMenu.Strings>("LIGHTS_OUT");

	public Type ParentMenu => typeof(SideMenuMenuHappyBaldiModifiersGeneral);
}
