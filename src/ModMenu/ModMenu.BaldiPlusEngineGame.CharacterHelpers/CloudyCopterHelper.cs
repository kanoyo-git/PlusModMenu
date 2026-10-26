using System.Collections.Generic;
using System.Reflection;
using PlusModMenu.Core.Common.Extensions;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.CharacterHelpers;

internal class CloudyCopterHelper : MonoBehaviour
{
	internal class WindTriggerIdentifier : MonoBehaviour
	{
	}

	private static readonly FieldInfo _windManager = AccessTools.Field(typeof(Cumulo), "windManager");

	private static readonly FieldInfo _windSpeed = AccessTools.Field(typeof(Cumulo), "windSpeed");

	private Cumulo _cloudyCopterInstance;

	public static List<CloudyCopterHelper> HelperInstances { get; } = new List<CloudyCopterHelper>();

	public BeltManager WindManager { get; private set; }

	public float WindSpeed
	{
		get
		{
			return _windSpeed.GetValue<float>(_cloudyCopterInstance);
		}
		set
		{
			_windSpeed.SetValue(_cloudyCopterInstance, value);
			WindManager.SetSpeed(value);
		}
	}

	private void Awake()
	{
		_cloudyCopterInstance = GetComponent<Cumulo>();
		WindManager = _windManager.GetValue<BeltManager>(_cloudyCopterInstance);
		WindManager.gameObject.AddComponent<WindTriggerIdentifier>();
		HelperInstances.Add(this);
		CloudyCopterModifiers cloudyCopterModifiers = Service.Get<IModMenuService>().Get<CloudyCopterModifiers>();
		WindSpeed = cloudyCopterModifiers.WindForceSpeed;
	}

	private void OnEnable()
	{
		if (_cloudyCopterInstance != null && _cloudyCopterInstance.behaviorStateMachine.CurrentState is Cumulo_Blowing)
		{
			WindManager.gameObject.SetActive(value: true);
		}
	}

	private void OnDisable()
	{
		if (WindManager != null)
		{
			WindManager.gameObject.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		HelperInstances.Remove(this);
	}
}
