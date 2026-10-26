using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Collectors;
using PlusModMenu.Api.Collectors;
using PlusModMenu.Api.Interfaces;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables;

[DataContract]
internal abstract class AbstractEventModifiers<TEvent> : AbstractEventModifiers, IModifiers where TEvent : RandomEvent
{
	private TEvent? _eventTemplate;

	private float _minEventTime = 30f;

	private float _maxEventTime = 60f;

	protected TEvent? EventTemplate
	{
		get
		{
			if (_eventTemplate == null)
			{
				return _eventTemplate = ModMenuManager.Events.FirstOrDefault((RandomEvent y) => y is TEvent) as TEvent;
			}
			return _eventTemplate;
		}
	}

	public virtual float MinEventTime
	{
		get
		{
			return _minEventTime;
		}
		set
		{
			_minEventTime = value;
			TEvent val = ModMenuManager.Events.FirstOrDefault((RandomEvent y) => y is TEvent) as TEvent;
			if (val != null)
			{
				AbstractEventModifiers._minEventTimeField.SetValue(val, MinEventTime);
			}
		}
	}

	public virtual float MaxEventTime
	{
		get
		{
			return _maxEventTime;
		}
		set
		{
			_maxEventTime = value;
			TEvent val = ModMenuManager.Events.FirstOrDefault((RandomEvent y) => y is TEvent) as TEvent;
			if (val != null)
			{
				AbstractEventModifiers._maxEventTimeField.SetValue(val, MaxEventTime);
			}
		}
	}

	public virtual void StartEvent()
	{
		RandomEvent randomEvent = ModMenuManager.Events.FirstOrDefault((RandomEvent x) => x.GetType() == typeof(TEvent));
		if (!(randomEvent == null))
		{
			RandomEvent randomEvent2 = UnityEngine.Object.Instantiate(randomEvent);
			System.Random controlledRNG = UnityEngine.Object.FindObjectOfType<LevelBuilder>()?.controlledRNG ?? new System.Random();
			randomEvent2.Initialize(Singleton<BaseGameManager>.Instance.Ec, controlledRNG);
			randomEvent2.SetEventTime(controlledRNG);
			randomEvent2.AfterUpdateSetup(controlledRNG);
			randomEvent2.Begin();
		}
	}

	protected void ApplyToAll()
	{
		foreach (EventCollectorItem item in MonoBehaviourCollector<TEvent, EventCollectorItem>.Collection)
		{
			ApplyTo(item.Item);
		}
	}

	public virtual void StopEvent()
	{
		foreach (EventCollectorItem item in MonoBehaviourCollector<TEvent, EventCollectorItem>.Collection.ToList())
		{
			if (AbstractEventModifiers._eventTimerField.GetValue(item.Item) is IEnumerator eventTimer)
			{
				item.Item.StopCoroutine(eventTimer);
			}
			item.Item.End();
		}
	}
}
internal abstract class AbstractEventModifiers
{
	internal virtual void ApplyTo(RandomEvent randomEvent)
	{
	}

	protected static void Set(RandomEvent randomEvent, string field, object value)
	{
		AccessTools.Field(randomEvent.GetType(), field)?.SetValue(randomEvent, value);
	}

	protected static void EnsureSpawnRoom()
	{
		var ec = Singleton<BaseGameManager>.Instance != null ? Singleton<BaseGameManager>.Instance.Ec : null;
		if (ec == null)
		{
			return;
		}
		ec.tempOpenEventSpawnerDoors ??= () => { };
		ec.tempCloseEventSpawnerDoors ??= () => { };
		if (ec.eventSpawnRooms.Count > 0)
		{
			return;
		}
		var rooms = ec.rooms.Where(r => r.category == RoomCategory.Class).ToList();
		if (rooms.Count == 0)
		{
			rooms = ec.rooms.Where(r => r.category != RoomCategory.Hall && r.category != RoomCategory.Null).ToList();
		}
		if (rooms.Count > 0)
		{
			ec.eventSpawnRooms.Add(rooms[UnityEngine.Random.Range(0, rooms.Count)]);
		}
	}

	[HarmonyPatch(typeof(RandomEvent), nameof(RandomEvent.Initialize))]
	private class InitializePatch
	{
		private static void Postfix(RandomEvent __instance)
		{
			foreach (var modifiers in PlusModMenu.Api.Service.Get<PlusModMenu.Api.Interfaces.Services.IModMenuService>().Modifiers.OfType<AbstractEventModifiers>())
			{
				modifiers.ApplyTo(__instance);
			}
		}
	}

	protected static readonly FieldInfo _minEventTimeField = AccessTools.Field(typeof(RandomEvent), "minEventTime");

	protected static readonly FieldInfo _maxEventTimeField = AccessTools.Field(typeof(RandomEvent), "maxEventTime");

	protected static readonly FieldInfo _eventTimerField = AccessTools.Field(typeof(RandomEvent), "eventTimer");
}
