using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuGroupOtherModifiersStickersSlots : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("STICKER_SLOTS");

	public Type ParentMenu => typeof(SideMenuMenuOtherModifiersStickers);
}
