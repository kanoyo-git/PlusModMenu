using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuGroupOtherModifiersWorldFloor : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("FLOOR");

	public Type ParentMenu => typeof(SideMenuMenuOtherModifiersWorld);
}
