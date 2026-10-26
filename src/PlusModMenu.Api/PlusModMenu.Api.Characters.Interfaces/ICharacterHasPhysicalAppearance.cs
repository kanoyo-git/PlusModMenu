using PlusModMenu.Api.Interfaces;

namespace PlusModMenu.Api.Characters.Interfaces;

public interface ICharacterHasPhysicalAppearance : ICharacterModifiers, IModifiers
{
	float Scale { get; set; }

	bool IsInvisible { get; set; }
}
