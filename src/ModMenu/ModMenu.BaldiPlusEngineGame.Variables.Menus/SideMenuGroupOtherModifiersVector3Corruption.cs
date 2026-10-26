using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuGroupOtherModifiersVector3Corruption : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("VECTOR3_CORRUPTION");

	public Type ParentMenu => typeof(SideMenuMenuOtherModifiersTrippyModifiers);
}
