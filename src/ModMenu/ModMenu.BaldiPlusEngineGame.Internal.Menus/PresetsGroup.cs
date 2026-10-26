using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;
using ModMenu.Settings;

namespace ModMenu.BaldiPlusEngineGame.Internal.Menus;

internal class PresetsGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("PRESET_CONTROL_PANEL");

	public Type? ParentMenu => typeof(PresetsMenu);
}
