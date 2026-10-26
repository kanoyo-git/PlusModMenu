using System.Linq;
using PlusModMenu.Core.Common.Extensions;
using HarmonyLib;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Debugging;

public class DebugHelper
{
	public static GameObject CreateDebug3DObject()
	{
		GravityFlipper gravityFlipper = Object.Instantiate(AccessTools.Field(typeof(GravityEvent), "flipperPre").GetValue<GravityFlipper[]>(ModMenuManager.Events.OfType<GravityEvent>().First())[1]);
		gravityFlipper.enabled = false;
		return gravityFlipper.gameObject;
	}
}
