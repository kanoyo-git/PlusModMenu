using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuGroupOtherModifiersWorldDoors : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("DOORS_AND_WINDOWS");

	public Type ParentMenu => typeof(SideMenuMenuOtherModifiersWorld);
}
