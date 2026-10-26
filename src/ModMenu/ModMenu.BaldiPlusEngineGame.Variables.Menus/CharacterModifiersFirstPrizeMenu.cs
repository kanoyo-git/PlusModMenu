using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class CharacterModifiersFirstPrizeMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("FIRST_PRIZE");

	public Type ParentGroup => typeof(CharacterModifiersMenuGroup);
}
