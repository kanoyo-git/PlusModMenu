using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class PlayerModifiersCameraGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("CAMERA");

	public Type ParentMenu => typeof(PlayerModifiersGeneralMenu);
}
