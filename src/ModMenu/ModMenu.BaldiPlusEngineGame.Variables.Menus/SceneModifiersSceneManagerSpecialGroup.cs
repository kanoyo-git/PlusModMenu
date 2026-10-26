using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SceneModifiersSceneManagerSpecialGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("SPECIAL");

	public Type ParentMenu => typeof(SceneModifiersSceneManagerMenu);
}
