using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal record SideMenuGroupBalderDashEventModifiersGeneral : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("GENERAL");

	public Type ParentMenu => typeof(SideMenuBalderDashEventModifiers);
}
