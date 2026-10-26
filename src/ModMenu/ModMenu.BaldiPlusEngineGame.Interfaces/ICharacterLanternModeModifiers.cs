using PlusModMenu.Api.Characters.Interfaces;
using PlusModMenu.Api.Interfaces;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Interfaces;

public interface ICharacterLanternModeModifiers : ICharacterModifiers, IModifiers, ILanternModeSourceModifiers
{
	new bool EmitsLightDuringLightsOut { get; set; }

	new Color LightsOutLightColor { get; set; }

	new float LightsOutStrength { get; set; }
}
