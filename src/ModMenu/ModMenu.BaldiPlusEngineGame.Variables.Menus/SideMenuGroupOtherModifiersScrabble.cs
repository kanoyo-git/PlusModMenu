using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuGroupOtherModifiersScrabble : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("SCRABBLE");

	public Type ParentMenu => typeof(SideMenuMenuOtherModifiersTrippyModifiers);
}
