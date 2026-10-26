using System;
using System.Collections;
using System.Collections.Generic;
using PlusModMenu.Core.Common.Extensions;
using PlusModMenu.Core.Unity;
using HarmonyLib;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.CharacterHelpers;

public class PausableCoroutineHelper : MonoBehaviour
{
	private static readonly Dictionary<MonoBehaviour, PausableCoroutineHelper> _characterRoutines = new Dictionary<MonoBehaviour, PausableCoroutineHelper>();

	private readonly List<TaskManager.Task> _coroutines = new List<TaskManager.Task>();

	private MonoBehaviour _characterInstance;

	public static Coroutine StartCoroutine(MonoBehaviour @this, IEnumerator routine)
	{
		if (!@this.gameObject.activeSelf)
		{
			return @this.StartCoroutine(routine);
		}
		if (!_characterRoutines.ContainsKey(@this))
		{
			PausableCoroutineHelper pausableCoroutineHelper = @this.gameObject.AddComponent<PausableCoroutineHelper>();
			pausableCoroutineHelper._characterInstance = @this;
			_characterRoutines[@this] = pausableCoroutineHelper;
		}
		PausableCoroutineHelper coroutineHelper = _characterRoutines[@this];
		TaskManager.Task task = new TaskManager.Task(routine);
		task.Finished += (bool _) =>
		{
			coroutineHelper._coroutines.Remove(task);
		};
		coroutineHelper._coroutines.Add(task);
		return task.Start();
	}

	public static Coroutine StartCoroutine(MonoBehaviour @this, string methodName)
	{
		return StartCoroutine(@this, AccessTools.Method(@this.GetType(), methodName).Invoke<IEnumerator>(@this, Array.Empty<object>()));
	}

	public static void StopAllCoroutines(MonoBehaviour @this)
	{
		if (_characterRoutines.ContainsKey(@this))
		{
			PausableCoroutineHelper pausableCoroutineHelper = _characterRoutines[@this];
			foreach (TaskManager.Task coroutine in pausableCoroutineHelper._coroutines)
			{
				coroutine.Stop();
			}
			pausableCoroutineHelper._coroutines.Clear();
			_characterRoutines.Remove(pausableCoroutineHelper._characterInstance);
		}
		@this.StopAllCoroutines();
	}

	private void OnEnable()
	{
		foreach (TaskManager.Task coroutine in _coroutines)
		{
			coroutine.Resume();
		}
	}

	private void OnDisable()
	{
		foreach (TaskManager.Task coroutine in _coroutines)
		{
			coroutine.Pause();
		}
	}

	private void OnDestroy()
	{
		foreach (TaskManager.Task coroutine in _coroutines)
		{
			coroutine.Stop();
		}
		_coroutines.Clear();
		_characterRoutines.Remove(_characterInstance);
	}
}
