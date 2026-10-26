using System.Collections;
using System.Collections.Generic;
using PlusModMenu.Core.Unity;
using PlusModMenu.Api;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables.OtherModifiers;

public class DiscoManager
{
	private static readonly Dictionary<Cell, Color> _originalColors = new Dictionary<Cell, Color>();

	private readonly TaskManager.SimpleTask _task;

	private bool _enabled;

	public bool Enabled
	{
		get
		{
			return _enabled;
		}
		set
		{
			_enabled = value;
			if (value)
			{
				_task.Reset();
				_task.Start();
				Service.Get<IEventService>().Subscribe<ILevelStartedEvent>(Reset);
				return;
			}
			_task.Stop();
			foreach (KeyValuePair<Cell, Color> originalColor in _originalColors)
			{
				originalColor.Key.lightColor = originalColor.Value;
			}
			UpdateLighting();
			Service.Get<IEventService>().Unsubscribe<ILevelStartedEvent>(Reset);
		}
	}

	public float MinCooldown { get; set; }

	public float MaxCooldown { get; set; } = 1f;

	internal DiscoManager()
	{
		_task = new TaskManager.SimpleTask(Routine);
	}

	private static void Reset(object? _ = null)
	{
		_originalColors.Clear();
		if (Singleton<BaseGameManager>.Instance == null)
		{
			return;
		}
		foreach (Cell light in Singleton<BaseGameManager>.Instance.Ec.lights)
		{
			_originalColors.Add(light, light.lightColor);
		}
	}

	public IEnumerator Routine()
	{
		Reset();
		while (true)
		{
			if (!Singleton<BaseGameManager>.Instance || Singleton<BaseGameManager>.Instance.Ec == null)
			{
				yield return null;
				continue;
			}
			foreach (Cell light in Singleton<BaseGameManager>.Instance.Ec.lights)
			{
				light.lightColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
			}
			UpdateLighting();
			yield return new WaitForSeconds(Random.Range(MinCooldown, MaxCooldown));
		}
	}

	private static void UpdateLighting()
	{
		if (!Singleton<BaseGameManager>.Instance)
		{
			return;
		}
		EnvironmentController ec = Singleton<BaseGameManager>.Instance.Ec;
		if (ec == null)
		{
			return;
		}
		foreach (Cell light in ec.lights)
		{
			ec.UpdateLightingAtCell(light);
		}
	}
}
