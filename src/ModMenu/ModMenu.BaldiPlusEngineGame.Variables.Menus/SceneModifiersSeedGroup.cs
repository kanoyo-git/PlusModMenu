using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SceneModifiersSeedGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("SEED");

	public Type ParentMenu => typeof(SceneModifiersGeneralMenu);
}
