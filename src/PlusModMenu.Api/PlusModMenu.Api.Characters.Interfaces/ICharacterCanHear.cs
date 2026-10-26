using PlusModMenu.Api.Interfaces;

namespace PlusModMenu.Api.Characters.Interfaces;

public interface ICharacterCanHear : ICharacterModifiers, IModifiers
{
	bool IsDeaf { get; set; }
}
