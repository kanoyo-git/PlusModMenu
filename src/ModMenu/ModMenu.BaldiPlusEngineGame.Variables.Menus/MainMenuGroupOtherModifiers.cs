using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;
using ModMenu.Settings;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

[Order(30, OrderAttribute.OrderMode.BeforeUnordered)]
internal class MainMenuGroupOtherModifiers : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("OTHER_MODIFIERS");

	public Type? ParentMenu => null;
}
