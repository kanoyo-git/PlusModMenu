using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal record SideMenuGroupGravityEventModifiersGeneral : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("GENERAL");

	public Type ParentMenu => typeof(SideMenuGravityEventModifiers);
}
