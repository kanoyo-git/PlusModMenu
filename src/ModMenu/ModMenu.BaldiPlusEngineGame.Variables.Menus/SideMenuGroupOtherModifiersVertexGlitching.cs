using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuGroupOtherModifiersVertexGlitching : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("VERTEX_GLITCHING");

	public Type ParentMenu => typeof(SideMenuMenuOtherModifiersTrippyModifiers);
}
