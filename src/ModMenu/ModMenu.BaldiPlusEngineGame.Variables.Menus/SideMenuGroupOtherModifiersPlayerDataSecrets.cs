using System;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.UI;

namespace ModMenu.BaldiPlusEngineGame.Variables.Menus;

internal class SideMenuGroupOtherModifiersPlayerDataSecrets : IStaticGroup
{
	public LabelAttribute Label => new LabelAttribute<Strings>("SECRETS");

	public Type ParentMenu => typeof(SideMenuMenuOtherModifiersPlayerData);
}
