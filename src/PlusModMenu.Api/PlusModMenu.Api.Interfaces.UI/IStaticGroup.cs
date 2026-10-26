using System;
using PlusModMenu.Api.Attributes;

namespace PlusModMenu.Api.Interfaces.UI;

public interface IStaticGroup
{
	LabelAttribute Label { get; }

	Type? ParentMenu { get; }
}
