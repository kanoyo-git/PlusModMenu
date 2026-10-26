using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class CharacterModifiersBaldiMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("BALDI");

	public Type ParentGroup => typeof(CharacterModifiersMenuGroup);
}
