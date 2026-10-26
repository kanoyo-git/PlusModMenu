using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class CharacterModifiersMrsPompMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("MRS_POMP");

	public Type ParentGroup => typeof(CharacterModifiersMenuGroup);
}
