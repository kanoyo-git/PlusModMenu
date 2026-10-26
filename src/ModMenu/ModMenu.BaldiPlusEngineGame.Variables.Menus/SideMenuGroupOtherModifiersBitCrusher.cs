using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuGroupOtherModifiersBitCrusher : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("BIT_CRUSHER");

	public Type ParentMenu => typeof(SideMenuMenuOtherModifiersAudioManipulation);
}
