using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuGroupOtherModifiersWorldFieldTrips : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("FIELD_TRIPS");

	public Type ParentMenu => typeof(SideMenuMenuOtherModifiersWorld);
}
