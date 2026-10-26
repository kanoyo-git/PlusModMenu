using PlusModMenu.Api.Interfaces;

namespace PlusModMenu.Api.Characters.Interfaces;

public interface ICharacterCanBeToggled : ICharacterModifiers, IModifiers
{
	bool Enabled { get; set; }
}
