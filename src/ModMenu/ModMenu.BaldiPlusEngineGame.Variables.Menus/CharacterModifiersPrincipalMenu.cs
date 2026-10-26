using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class CharacterModifiersPrincipalMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("PRINCIPAL_OF_THE_THING");

	public Type ParentGroup => typeof(CharacterModifiersMenuGroup);
}
