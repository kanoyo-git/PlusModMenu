using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;
using ModMenu.Settings;

namespace ModMenu.BaldiPlusEngineGame.Internal.Menus;

[Order(100, OrderAttribute.OrderMode.AfterUnordered)]
internal class PresetsSection : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("PRESETS");

	public Type? ParentMenu => null;
}
