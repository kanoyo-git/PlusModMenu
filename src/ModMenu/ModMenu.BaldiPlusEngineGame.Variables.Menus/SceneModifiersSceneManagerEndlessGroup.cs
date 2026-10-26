using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SceneModifiersSceneManagerEndlessGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("ENDLESS_MODE");

	public Type ParentMenu => typeof(SceneModifiersSceneManagerMenu);
}
