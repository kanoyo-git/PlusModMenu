using System;
using ModMenu.Settings;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

[Order(0, OrderAttribute.OrderMode.BeforeUnordered)]
internal class CharacterModifiersAllMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("ALL_CHARACTERS");

	public Type ParentGroup => typeof(CharacterModifiersMenuGroup);
}
