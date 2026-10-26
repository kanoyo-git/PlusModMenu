using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuGroupOtherModifiersStickersInventory : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("STICKER_INVENTORY");

	public Type ParentMenu => typeof(SideMenuMenuOtherModifiersStickers);
}
