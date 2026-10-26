using HarmonyLib;
using ModMenu.Hl1;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Hl1;

internal sealed class PlayerBody
{
	private static readonly AccessTools.FieldRef<Entity, bool> Overridden = AccessTools.FieldRefAccess<Entity, bool>("overridden");

	private readonly PlayerManager _player;

	private readonly CameraModifier _camera = new(Vector3.zero, Vector3.zero);

	private bool _attached;

	public PlayerBody(PlayerManager player)
	{
		_player = player;
	}

	public PlayerManager Player => _player;

	public float CameraOffset => _camera.posOffset.y;

	public void Apply(float feet, float eye)
	{
		Attach();
		_camera.posOffset.y = feet + eye - Hl1Constants.StandEye;
		_player.plm.Entity.SetGrounded(feet <= 0.01f);
	}

	public void Restore()
	{
		_camera.posOffset = Vector3.zero;
		if (_player != null)
		{
			_player.plm.Entity.SetGrounded(true);
			_player.camMods.Remove(_camera);
			Singleton<CoreGameManager>.Instance?.GetCamera(_player.playerNumber)?.cameraModifiers.Remove(_camera);
		}
		_attached = false;
	}

	public bool GameControls
	{
		get
		{
			var entity = _player.plm.Entity;
			var camera = Singleton<CoreGameManager>.Instance.GetCamera(_player.playerNumber);
			return Time.timeScale == 0 || entity.Frozen || entity.InteractionDisabled || Overridden(entity) || camera == null || !camera.Controllable;
		}
	}

	public string GameControlsReason
	{
		get
		{
			var entity = _player.plm.Entity;
			var camera = Singleton<CoreGameManager>.Instance.GetCamera(_player.playerNumber);
			return $"timeScale {Time.timeScale}, frozen {entity.Frozen}, interaction disabled {entity.InteractionDisabled}, overridden {Overridden(entity)}, camera controllable {camera != null && camera.Controllable}";
		}
	}

	private void Attach()
	{
		if (_attached) return;
		_player.camMods.Add(_camera);
		Singleton<CoreGameManager>.Instance.GetCamera(_player.playerNumber).cameraModifiers.Add(_camera);
		_attached = true;
	}
}
