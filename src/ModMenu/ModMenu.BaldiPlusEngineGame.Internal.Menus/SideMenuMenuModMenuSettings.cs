using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;
using ModMenu.Settings;

namespace ModMenu.BaldiPlusEngineGame.Internal.Menus;

[Order(0, OrderAttribute.OrderMode.BeforeUnordered)]
internal class SideMenuMenuModMenuSettings : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("SETTINGS");

	public Type ParentGroup => typeof(MainMenuGroupModMenu);
}
