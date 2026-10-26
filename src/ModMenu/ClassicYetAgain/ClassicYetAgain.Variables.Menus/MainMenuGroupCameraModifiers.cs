using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;
using ModMenu.Settings;

namespace ClassicYetAgain.Variables.Menus;

[Order(26, OrderAttribute.OrderMode.BeforeUnordered)]
internal record MainMenuGroupCameraModifiers : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("CAMERA_MODIFIERS");

	public Type? ParentMenu => null;
}
