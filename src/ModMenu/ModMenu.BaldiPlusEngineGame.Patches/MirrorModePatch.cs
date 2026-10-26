using System.Reflection;
using PlusModMenu.Core.Common.Extensions;
using HarmonyLib;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class MirrorModePatch
{
	[HarmonyPatch(typeof(MirrorMode), "Initialize")]
	private class Initialize
	{
		private static bool Prefix(MirrorMode __instance, bool ___active)
		{
			if (___active)
			{
				return false;
			}
			if (MirrorModeHelper.Instance == null)
			{
				__instance.gameObject.AddComponent<MirrorModeHelper>().MirrorMode = __instance;
			}
			else if (MirrorModeHelper.Instance.MirrorMode != __instance)
			{
				Object.Destroy(__instance);
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(MirrorMode), "OnDisable")]
	private class OnDisable
	{
		private static readonly FieldInfo _audioReversed = AccessTools.Field(typeof(GameCamera), "audioReversed");

		private static void Prefix(bool ___active, Camera[] ___cameraToMirror)
		{
			if (!___active)
			{
				return;
			}
			foreach (Camera camera in ___cameraToMirror)
			{
				if (!(camera == null))
				{
					camera.projectionMatrix *= Matrix4x4.Scale(new Vector3(-1f, 1f, 1f));
				}
			}
			GL.invertCulling = false;
			if (!(Singleton<SubtitleManager>.Instance == null) && Singleton<SubtitleManager>.Instance.Reversed)
			{
				Singleton<SubtitleManager>.Instance.Reverse();
			}
		}

		private static void Postfix()
		{
			if (!(Singleton<CoreGameManager>.Instance == null) && !(Singleton<CoreGameManager>.Instance.GetCamera(0) == null))
			{
				GameCamera camera = Singleton<CoreGameManager>.Instance.GetCamera(0);
				if (_audioReversed.GetValue<bool>(camera))
				{
					camera.ReverseAudio();
				}
				PlayerManager player = Singleton<CoreGameManager>.Instance.GetPlayer(0);
				if (!(player == null))
				{
					player.reversed = false;
				}
			}
		}
	}

	internal class MirrorModeHelper : MonoBehaviour
	{
		public static MirrorModeHelper? Instance { get; private set; }

		public MirrorMode? MirrorMode { get; set; }

		public bool Enabled
		{
			get
			{
				return MirrorMode?.enabled ?? false;
			}
			set
			{
				if (MirrorMode != null)
				{
					MirrorMode.enabled = value;
				}
				enabled = value;
			}
		}

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
		}

		private void Start()
		{
			if (Instance != this)
			{
				Object.Destroy(this);
			}
		}

		private void OnDestroy()
		{
			if (Instance == this)
			{
				Instance = null;
			}
		}

		private void OnEnable()
		{
			MirrorMode?.Initialize();
		}
	}
}
