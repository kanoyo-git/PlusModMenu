using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Interfaces;

public interface ILanternModeSourceModifiers
{
	bool EmitsLightDuringLightsOut { get; set; }

	Color LightsOutLightColor { get; set; }

	float LightsOutStrength { get; set; }
}
