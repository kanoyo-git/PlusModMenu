using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SceneModifiersSceneManagerOtherGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("OTHER");

	public Type ParentMenu => typeof(SceneModifiersSceneManagerMenu);
}
