using System;
using System.Collections;
using UnityEngine;

namespace PlusModMenu.Core.Unity;

public class TaskManager : MonoBehaviour
{
	public class SimpleTask
	{
		private readonly Func<IEnumerator>? _coroutineFunc;

		protected IEnumerator? _coroutine;

		public Coroutine? RunningCoroutine { get; protected set; }

		public SimpleTask(IEnumerator coroutine)
		{
			_coroutine = coroutine;
		}

		public SimpleTask(Func<IEnumerator> coroutineFunc)
		{
			_coroutineFunc = coroutineFunc;
			Reset();
		}

		public virtual Coroutine Start()
		{
			return RunningCoroutine = _instance.StartCoroutine(_coroutine);
		}

		public virtual void Stop()
		{
			_instance.StopCoroutine(_coroutine);
		}

		public virtual void Reset()
		{
			if (_coroutineFunc != null)
			{
				_coroutine = _coroutineFunc();
			}
		}
	}

	public class Task : SimpleTask
	{
		public delegate void FinishedHandler(bool aborted);

		private bool _stopped;

		public bool Running { get; private set; }

		public bool Paused { get; private set; }

		public event FinishedHandler? Finished;

		public Task(IEnumerator coroutine)
			: base(coroutine)
		{
		}

		public Task(Func<IEnumerator> coroutineFunc)
			: base(coroutineFunc)
		{
		}

		public override Coroutine Start()
		{
			return Start(resumeIfAlreadyRunning: false);
		}

		public Coroutine Start(bool resumeIfAlreadyRunning)
		{
			if (resumeIfAlreadyRunning && Running)
			{
				if (Paused)
				{
					Resume();
				}
				return RunningCoroutine;
			}
			Running = true;
			return RunningCoroutine = _instance.StartCoroutine(CallWrapper());
		}

		public override void Stop()
		{
			_stopped = true;
			Running = false;
		}

		public void Pause()
		{
			Paused = true;
		}

		public void Resume()
		{
			Paused = false;
		}

		private IEnumerator CallWrapper()
		{
			IEnumerator e = _coroutine;
			while (Running)
			{
				if (Paused)
				{
					yield return null;
				}
				else if (e != null && e.MoveNext())
				{
					yield return e.Current;
				}
				else
				{
					Running = false;
				}
			}
			Finished?.Invoke(_stopped);
		}
	}

	private static readonly TaskManager _instance;

	static TaskManager()
	{
		GameObject gameObject = new GameObject("TaskManager");
		_instance = gameObject.AddComponent<TaskManager>();
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
	}
}
