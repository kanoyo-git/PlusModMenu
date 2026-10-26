using System;
using System.Collections.Generic;
using PlusModMenu.Api.Interfaces;

namespace PlusModMenu.Api.Characters.Interfaces;

public interface IBasicCharacter : ICharacterCanBeToggled, ICharacterModifiers, IModifiers
{
	Type Type { get; }

	bool Exists { get; }

	IReadOnlyList<CharacterCollectorItem> Collection { get; }
}
