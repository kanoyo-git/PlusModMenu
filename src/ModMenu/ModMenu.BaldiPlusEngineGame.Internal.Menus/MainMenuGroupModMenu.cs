using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;
using ModMenu.Settings;

namespace ModMenu.BaldiPlusEngineGame.Internal.Menus;

[Order(101, OrderAttribute.OrderMode.AfterUnordered)]
internal class MainMenuGroupModMenu : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("MENU_SETTINGS");

	public Type? ParentMenu => null;
}
