using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal record SideMenuPartyEventModifiers : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("PARTY_EVENT");

	public Type ParentGroup => typeof(MainMenuGroupEventModifiers);
}
