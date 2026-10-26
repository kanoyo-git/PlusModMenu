using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class ITM_AlarmClockPatch
{
	[HarmonyPatch(typeof(ITM_AlarmClock), "Use")]
	private class Use
	{
		private static void Prefix(float[] ___setTime)
		{
			float alarmTimerMultiplier = Service.Get<IModMenuService>().Get<AlarmClockModifiers>().AlarmTimerMultiplier;
			for (int i = 0; i < ___setTime.Length; i++)
			{
				___setTime[i] *= alarmTimerMultiplier;
			}
		}
	}
}
