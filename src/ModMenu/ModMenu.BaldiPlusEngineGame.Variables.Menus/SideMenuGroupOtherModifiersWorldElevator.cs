using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuGroupOtherModifiersWorldElevator : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("ELEVATOR");

	public Type ParentMenu => typeof(SideMenuMenuOtherModifiersWorld);
}
