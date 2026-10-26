using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuGroupOtherModifiersWorldMap : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("MAP");

	public Type ParentMenu => typeof(SideMenuMenuOtherModifiersWorld);
}
