using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class PlayerModifiersSurvivalGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("SURVIVAL");

	public Type ParentMenu => typeof(PlayerModifiersGeneralMenu);
}
