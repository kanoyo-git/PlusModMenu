using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SceneModifiersSceneManagerMainGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("MAIN_FLOORS");

	public Type ParentMenu => typeof(SceneModifiersSceneManagerMenu);
}
