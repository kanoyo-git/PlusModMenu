using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;
using ModMenu.Settings;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

[Order(20, OrderAttribute.OrderMode.BeforeUnordered)]
internal class GameplayModifiersMenuGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("GAMEPLAY_MODIFIERS");

	public Type? ParentMenu => null;
}
