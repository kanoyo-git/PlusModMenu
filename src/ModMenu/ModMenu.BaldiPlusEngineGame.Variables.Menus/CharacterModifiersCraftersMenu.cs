using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class CharacterModifiersCraftersMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("ARTS_AND_CRAFTERS");

	public Type ParentGroup => typeof(CharacterModifiersMenuGroup);
}
