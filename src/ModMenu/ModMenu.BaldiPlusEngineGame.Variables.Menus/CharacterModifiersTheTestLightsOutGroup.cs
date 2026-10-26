using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class CharacterModifiersTheTestLightsOutGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("LIGHTS_OUT");

	public Type ParentMenu => typeof(CharacterModifiersTheTestMenu);
}
