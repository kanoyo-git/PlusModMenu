using PlusModMenu.Api.Interfaces;

namespace PlusModMenu.Api.Characters.Interfaces;

public interface ICharacterCanMove : ICharacterModifiers, IModifiers
{
	float Speed { get; set; }
}
