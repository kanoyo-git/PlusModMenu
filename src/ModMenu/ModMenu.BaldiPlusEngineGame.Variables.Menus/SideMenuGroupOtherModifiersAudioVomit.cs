using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuGroupOtherModifiersAudioVomit : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("AUDIO_VOMIT");

	public Type ParentMenu => typeof(SideMenuMenuOtherModifiersTrippyModifiers);
}
