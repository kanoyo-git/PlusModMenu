using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class ItemModifiersSafetyScissorsMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("SAFETY_SCISSORS");

	public Type ParentGroup => typeof(ItemModifiersMenuGroup);
}
