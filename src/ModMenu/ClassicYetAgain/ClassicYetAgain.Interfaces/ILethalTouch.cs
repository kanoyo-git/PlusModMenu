using PlusModMenu.Api.Characters.Interfaces;
using PlusModMenu.Api.Interfaces;

namespace ClassicYetAgain.Interfaces;

internal interface ILethalTouch : ICharacterModifiers, IModifiers
{
	bool LethalTouch { get; set; }
}
