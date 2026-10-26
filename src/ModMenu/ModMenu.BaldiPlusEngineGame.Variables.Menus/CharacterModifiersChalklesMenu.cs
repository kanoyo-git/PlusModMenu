using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class CharacterModifiersChalklesMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("CHALKLES");

	public Type ParentGroup => typeof(CharacterModifiersMenuGroup);
}
