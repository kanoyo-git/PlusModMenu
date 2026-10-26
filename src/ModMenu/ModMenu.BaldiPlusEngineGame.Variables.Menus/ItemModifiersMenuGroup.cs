using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;
using ModMenu.Settings;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

[Order(5, OrderAttribute.OrderMode.BeforeUnordered)]
internal class ItemModifiersMenuGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("ITEM_MODIFIERS");

	public Type? ParentMenu => null;
}
