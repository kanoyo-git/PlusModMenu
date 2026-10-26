using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;
using ModMenu.Settings;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

[Order(35, OrderAttribute.OrderMode.BeforeUnordered)]
internal class TeleporterMenuGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("TELEPORTER");

	public Type? ParentMenu => null;
}
