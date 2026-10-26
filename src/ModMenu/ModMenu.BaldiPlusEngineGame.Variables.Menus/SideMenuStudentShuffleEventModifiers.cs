using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal record SideMenuStudentShuffleEventModifiers : IStaticMenu
{
	public LabelAttribute Label => new LabelAttribute<Strings>("STUDENT_SHUFFLE_EVENT");

	public Type ParentGroup => typeof(MainMenuGroupEventModifiers);
}
