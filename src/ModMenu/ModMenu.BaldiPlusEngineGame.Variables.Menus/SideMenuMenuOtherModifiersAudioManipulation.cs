using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuMenuOtherModifiersAudioManipulation : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("AUDIO_MODIFIERS");

	public Type ParentGroup => typeof(MainMenuGroupOtherModifiers);
}
