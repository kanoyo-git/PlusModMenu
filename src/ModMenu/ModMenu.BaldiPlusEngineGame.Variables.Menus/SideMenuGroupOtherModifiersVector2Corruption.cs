using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuGroupOtherModifiersVector2Corruption : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("VECTOR2_CORRUPTION");

	public Type ParentMenu => typeof(SideMenuMenuOtherModifiersTrippyModifiers);
}
