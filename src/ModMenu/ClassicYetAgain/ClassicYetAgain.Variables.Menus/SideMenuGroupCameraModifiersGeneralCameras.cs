using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ClassicYetAgain.Variables.Menus;

internal record SideMenuGroupCameraModifiersGeneralCameras : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("CAMERAS");

	public Type ParentMenu => typeof(SideMenuMenuCameraModifiersGeneral);
}
