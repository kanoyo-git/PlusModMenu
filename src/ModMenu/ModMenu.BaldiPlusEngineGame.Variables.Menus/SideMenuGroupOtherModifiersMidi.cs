using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuGroupOtherModifiersMidi : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("MIDI");

	public Type ParentMenu => typeof(SideMenuMenuOtherModifiersAudioManipulation);
}
