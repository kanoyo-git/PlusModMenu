using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class CharacterModifiersTheTestMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("THE_TEST");

	public Type ParentGroup => typeof(CharacterModifiersMenuGroup);
}
