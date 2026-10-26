using System;
using PlusModMenu.Api.Attributes;

namespace PlusModMenu.Api.Interfaces.UI;

public interface IStaticMenu
{
	LabelAttribute Label { get; }

	Type ParentGroup { get; }
}
