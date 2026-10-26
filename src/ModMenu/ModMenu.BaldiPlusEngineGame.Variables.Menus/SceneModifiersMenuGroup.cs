using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;
using ModMenu.Settings;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

[Order(25, OrderAttribute.OrderMode.BeforeUnordered)]
internal class SceneModifiersMenuGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("SCENE_MODIFIERS");

	public Type? ParentMenu => null;
}
