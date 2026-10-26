using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Internal.Menus;

internal class SideMenuGroupModMenuHotkeyControlPanel : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("HOTKEY_CONTROL_PANEL");

	public Type ParentMenu => typeof(SideMenuMenuHotkeys);
}
