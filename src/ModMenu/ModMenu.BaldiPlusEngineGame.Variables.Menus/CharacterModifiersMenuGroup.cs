using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;
using ModMenu.Settings;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

[Order(0, OrderAttribute.OrderMode.BeforeUnordered)]
internal class CharacterModifiersMenuGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("CHARACTER_MODIFIERS");

	public Type? ParentMenu => null;
}
