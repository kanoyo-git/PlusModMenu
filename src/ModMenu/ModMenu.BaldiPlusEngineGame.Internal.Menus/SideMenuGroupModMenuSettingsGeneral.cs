using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Internal.Menus;

internal class SideMenuGroupModMenuSettingsGeneral : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("GENERAL");

	public Type? ParentMenu => typeof(SideMenuMenuModMenuSettings);
}
