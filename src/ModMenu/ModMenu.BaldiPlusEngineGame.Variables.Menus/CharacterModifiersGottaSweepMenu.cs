using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class CharacterModifiersGottaSweepMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("GOTTA_SWEEP");

	public Type ParentGroup => typeof(CharacterModifiersMenuGroup);
}
