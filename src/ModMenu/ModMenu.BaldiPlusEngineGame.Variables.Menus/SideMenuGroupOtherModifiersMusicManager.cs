using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuGroupOtherModifiersMusicManager : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("MUSIC_MANAGER");

	public Type ParentMenu => typeof(SideMenuMenuOtherModifiersAudioManipulation);
}
