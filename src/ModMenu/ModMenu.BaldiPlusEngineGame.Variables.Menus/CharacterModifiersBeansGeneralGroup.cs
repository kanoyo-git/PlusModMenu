using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class CharacterModifiersBeansGeneralGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("GENERAL");

	public Type ParentMenu => typeof(CharacterModifiersBeansMenu);
}
