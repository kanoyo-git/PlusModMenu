using PlusModMenu.Api.Interfaces;

namespace PlusModMenu.Api.Characters.Interfaces;

public interface ICharacterCanSee : ICharacterModifiers, IModifiers
{
	bool IsBlind { get; set; }
}
