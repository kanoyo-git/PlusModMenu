using System.Collections;
using System.Linq;
using System.Reflection;
using PlusModMenu.Core.Common.Extensions;
using HarmonyLib;
using UnityEngine;

namespace ClassicYetAgain.Patches;

internal class CoreGameManagerPatch
{
	public class Helper
	{
		private static readonly MethodInfo _endSequence = AccessTools.Method(typeof(CoreGameManager), "EndSequence");

		public static void EndGame(Transform player, MonoBehaviour character)
		{
			if (ModMenu.BaldiPlusEngineGame.Patches.CoreGameManagerPatch.EndGameInProgress)
			{
				return;
			}
			CoreGameManager instance = Singleton<CoreGameManager>.Instance;
			Vector3 offestPos = (player.position - character.transform.position).normalized * 2f + Vector3.up + ModMenu.BaldiPlusEngineGame.Helper.DeathcamOffset.GetValueOrDefault(character.GetType(), Vector3.zero);
			Baldi baldi = ((Singleton<BaseGameManager>.Instance != null && Singleton<BaseGameManager>.Instance.Ec != null) ? Singleton<BaseGameManager>.Instance.Ec.GetBaldi() : null) ?? Resources.FindObjectsOfTypeAll<Baldi>().FirstOrDefault();
			if (baldi != null)
			{
				instance.EndGame(player, baldi);
				instance.GetCamera(0).UpdateTargets(character.transform, 0);
				instance.GetCamera(0).offestPos = offestPos;
				return;
			}
			Time.timeScale = 0f;
			Singleton<MusicManager>.Instance.StopMidi();
			instance.disablePause = true;
			instance.GetCamera(0).UpdateTargets(character.transform, 0);
			instance.GetCamera(0).offestPos = offestPos;
			instance.GetCamera(0).SetControllable(value: false);
			instance.GetCamera(0).matchTargetRotation = false;
			Singleton<StickerManager>.Instance.RemoveUnopenedStickerPacketsFromInventory();
			instance.audMan.volumeModifier = 0.6f;
			instance.StartCoroutine(_endSequence.Invoke<IEnumerator>(instance, new object[0]));
			Singleton<InputManager>.Instance.Rumble(1f, 2f);
		}
	}
}
