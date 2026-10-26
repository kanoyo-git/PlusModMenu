using System.Collections.Generic;
using System.Reflection;
using PlusModMenu.Core.Common.Extensions;
using HarmonyLib;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.CharacterHelpers;

internal class ArtsAndCraftersHelper : MonoBehaviour
{
	private static readonly FieldInfo _visibleRenderer = AccessTools.Field(typeof(ArtsAndCrafters), "visibleRenderer");

	private static readonly FieldInfo _audMan = AccessTools.Field(typeof(ArtsAndCrafters), "audMan");

	private static readonly FieldInfo _attackAudMan = AccessTools.Field(typeof(ArtsAndCrafters), "attackAudMan");

	private static readonly FieldInfo _echo = AccessTools.Field(typeof(ArtsAndCrafters), "echo");

	private static readonly FieldInfo _target = AccessTools.Field(typeof(ArtsAndCrafters), "target");

	private static readonly FieldInfo _angry = AccessTools.Field(typeof(ArtsAndCrafters), "angry");

	private static readonly FieldInfo _attacking = AccessTools.Field(typeof(ArtsAndCrafters), "attacking");

	private static readonly FieldInfo _running = AccessTools.Field(typeof(ArtsAndCrafters), "running");

	private static Sprite? _normalSprite;

	private ArtsAndCrafters _crafters;

	private SpriteRenderer _visibleRendererInstance;

	private int _normalLayer;

	private float _normalMaxSpeed;

	private float _normalAccel;

	private bool _disappeared;

	private void Awake()
	{
		_crafters = GetComponent<ArtsAndCrafters>();
		_visibleRendererInstance = _visibleRenderer.GetValue<SpriteRenderer>(_crafters);
		if (!(_normalSprite != null))
		{
			_normalSprite = _visibleRendererInstance.sprite;
		}
		_normalLayer = _visibleRendererInstance.gameObject.layer;
		_normalMaxSpeed = _crafters.Navigator.maxSpeed;
		_normalAccel = _crafters.Navigator.accel;
	}

	private void OnEnable()
	{
		if (_disappeared && _crafters != null)
		{
			_disappeared = false;
			_crafters.Navigator.Entity.SetActive(value: true);
		}
	}

	public void Reset()
	{
		_disappeared = true;
		_attacking.SetValue(_crafters, false);
		_target.SetValue(_crafters, null);
		_angry.SetValue(_crafters, false);
		_running.SetValue(_crafters, false);
		_visibleRendererInstance.sprite = _normalSprite;
		_visibleRendererInstance.gameObject.layer = _normalLayer;
		foreach (Transform item in _echo.GetValue<IEnumerable<Transform>>(_crafters))
		{
			item.gameObject.SetActive(value: false);
		}
		AudioManager audioManager = _attackAudMan.GetValue<AudioManager>(_crafters);
		audioManager.FlushQueue(endCurrent: true);
		audioManager.SetLoop(val: false);
		audioManager.maintainLoop = false;
		_audMan.GetValue<AudioManager>(_crafters).loop = false;
		_crafters.Navigator.maxSpeed = _normalMaxSpeed;
		_crafters.Navigator.accel = _normalAccel;
		_crafters.Navigator.SetSpeed(0f);
		_crafters.state = new ArtsAndCrafters_Waiting(_crafters);
		_crafters.behaviorStateMachine.ChangeState(_crafters.state);
	}
}
