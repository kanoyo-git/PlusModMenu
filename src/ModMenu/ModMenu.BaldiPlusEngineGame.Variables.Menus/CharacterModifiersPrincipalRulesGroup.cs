using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class CharacterModifiersPrincipalRulesGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("RULES");

	public Type ParentMenu => typeof(CharacterModifiersPrincipalMenu);
}
