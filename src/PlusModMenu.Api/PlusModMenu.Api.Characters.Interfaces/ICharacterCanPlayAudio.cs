using PlusModMenu.Api.Interfaces;

namespace PlusModMenu.Api.Characters.Interfaces;

public interface ICharacterCanPlayAudio : ICharacterModifiers, IModifiers
{
	bool IsSilent { get; set; }
}
