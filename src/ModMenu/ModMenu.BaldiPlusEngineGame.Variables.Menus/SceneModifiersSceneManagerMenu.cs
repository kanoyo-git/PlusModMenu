using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SceneModifiersSceneManagerMenu : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("SCENE_MANAGER");

	public Type ParentGroup => typeof(SceneModifiersMenuGroup);
}
