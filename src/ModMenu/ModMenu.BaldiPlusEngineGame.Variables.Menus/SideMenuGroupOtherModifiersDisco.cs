using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuGroupOtherModifiersDisco : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("DISCO");

	public Type ParentMenu => typeof(SideMenuMenuOtherModifiersTrippyModifiers);
}
