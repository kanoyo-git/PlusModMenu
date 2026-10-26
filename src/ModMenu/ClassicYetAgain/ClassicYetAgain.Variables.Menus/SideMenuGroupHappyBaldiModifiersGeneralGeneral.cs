using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ClassicYetAgain.Variables.Menus;

internal record SideMenuGroupHappyBaldiModifiersGeneralGeneral : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("GENERAL");

	public Type ParentMenu => typeof(SideMenuMenuHappyBaldiModifiersGeneral);
}
