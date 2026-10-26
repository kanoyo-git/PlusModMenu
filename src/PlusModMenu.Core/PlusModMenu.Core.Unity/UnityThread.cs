using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;

namespace PlusModMenu.Core.Unity;

public class UnityThread : MonoBehaviour
{
	private static readonly UnityThread _instance;

	private static readonly ConcurrentBag<Action> _actionQueue;

	static UnityThread()
	{
		_actionQueue = new ConcurrentBag<Action>();
		GameObject gameObject = new GameObject("UnityThread");
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		_instance = gameObject.AddComponent<UnityThread>();
	}

	public static void _()
	{
	}

	public static void ExecuteCoroutine(IEnumerator action)
	{
		ExecuteInUpdate(() =>
		{
			_instance.StartCoroutine(action);
		});
	}

	public static void ExecuteInUpdate(Action action)
	{
		_actionQueue.Add(action);
	}

	public void Update()
	{
		if (_actionQueue.Count >= 1)
		{
			Action result;
			while (_actionQueue.TryTake(out result))
			{
				result();
			}
		}
	}
}
