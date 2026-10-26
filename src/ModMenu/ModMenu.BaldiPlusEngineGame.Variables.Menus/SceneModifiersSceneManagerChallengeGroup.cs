using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SceneModifiersSceneManagerChallengeGroup : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("CHALLENGES");

	public Type ParentMenu => typeof(SceneModifiersSceneManagerMenu);
}
