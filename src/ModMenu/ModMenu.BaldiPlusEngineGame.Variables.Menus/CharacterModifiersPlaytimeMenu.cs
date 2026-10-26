using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class CharacterModifiersPlaytimeMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("PLAYTIME");

	public Type ParentGroup => typeof(CharacterModifiersMenuGroup);
}
