using PlusModMenu.Api.Interfaces;

namespace PlusModMenu.Api.Characters.Interfaces;

public interface ICharacterHasPlayerTrigger : ICharacterModifiers, IModifiers
{
	bool IgnoresPlayerTrigger { get; set; }
}
