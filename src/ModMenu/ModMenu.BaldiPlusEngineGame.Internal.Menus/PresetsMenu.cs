using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;
using ModMenu.Settings;

namespace ModMenu.BaldiPlusEngineGame.Internal.Menus;

internal class PresetsMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("PRESETS");

	public Type ParentGroup => typeof(PresetsSection);
}
