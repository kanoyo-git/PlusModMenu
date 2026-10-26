using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuGroupOtherModifiersSoundFont : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("SOUNDFONT");

	public Type ParentMenu => typeof(SideMenuMenuOtherModifiersAudioManipulation);
}
