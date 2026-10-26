using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;
using ModMenu.Settings;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

[Order(10, OrderAttribute.OrderMode.BeforeUnordered)]
internal record MainMenuGroupEventModifiers : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("EVENT_MODIFIERS");

	public Type? ParentMenu => null;
}
