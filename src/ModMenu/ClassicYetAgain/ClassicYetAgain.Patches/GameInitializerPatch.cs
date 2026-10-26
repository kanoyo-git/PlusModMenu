using ClassicYetAgain.Variables;
using HarmonyLib;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ClassicYetAgain.Patches;

internal class GameInitializerPatch
{
	[HarmonyPriority(Priority.Low)]
	[HarmonyPatch(typeof(GameInitializer), "Initialize")]
	private class Initialize
	{
		private struct SkyboxState
		{
			public Cubemap Cubemap { get; init; }

			public Color Color { get; init; }
		}

		private static void Prefix(ref SkyboxState? __state)
		{
			if (Service.Get<IModMenuService>().Get<OtherModifiers>().EnforceSelectedSkybox)
			{
				__state = new SkyboxState
				{
					Cubemap = (Cubemap)Shader.GetGlobalTexture("_Skybox"),
					Color = Shader.GetGlobalColor("_SkyboxColor")
				};
			}
		}

		private static void Postfix(SkyboxState? __state)
		{
			if (__state.HasValue)
			{
				Shader.SetGlobalTexture("_Skybox", __state.Value.Cubemap);
				Shader.SetGlobalColor("_SkyboxColor", __state.Value.Color);
			}
		}
	}
}
