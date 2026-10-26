using PlusModMenu.Api.Characters.Interfaces;
using PlusModMenu.Api.Interfaces;

namespace ClassicYetAgain.Interfaces;

public interface ICharacterGetsSpawnedInSpoopMode : ICharacterModifiers, IModifiers
{
	bool ActivateInSpoopMode { get; set; }
}
