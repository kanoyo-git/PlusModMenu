using System;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ClassicYetAgain.Variables.Menus;

internal record SideMenuGroupPrincipalModifiersGeneralDetentionTimes : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("CUSTOM_DETENTION_TIMES");

	public Type ParentMenu => typeof(CharacterModifiersPrincipalMenu);
}
