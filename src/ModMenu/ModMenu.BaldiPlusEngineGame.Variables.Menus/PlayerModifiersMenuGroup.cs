using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;
using ModMenu.Settings;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

[Order(15, OrderAttribute.OrderMode.BeforeUnordered)]
internal class PlayerModifiersMenuGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("PLAYER_MODIFIERS");

	public Type? ParentMenu => null;
}
