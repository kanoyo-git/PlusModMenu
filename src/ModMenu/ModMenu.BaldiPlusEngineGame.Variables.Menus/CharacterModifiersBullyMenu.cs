using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class CharacterModifiersBullyMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("ITS_A_BULLY");

	public Type ParentGroup => typeof(CharacterModifiersMenuGroup);
}
