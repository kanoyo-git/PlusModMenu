using System;
using System.Collections.Generic;
using System.Linq;
using PlusModMenu.Core.Common;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Services;

internal class EventService : IEventService, IService
{
	private readonly Dictionary<Type, IEventService.EventHandler> _events = new Dictionary<Type, IEventService.EventHandler>();

	public void Publish<TEvent>(object? args) where TEvent : IEvent
	{
		if (!_events.TryGetValue(typeof(TEvent), out IEventService.EventHandler value))
		{
			return;
		}
		List<Exception> list = new List<Exception>();
		foreach (IEventService.EventHandler item2 in value.GetInvocationList().Cast<IEventService.EventHandler>())
		{
			try
			{
				item2(args);
			}
			catch (Exception item)
			{
				list.Add(item);
			}
		}
		if (list.Any())
		{
			Debugger.Error(new AggregateException(list));
		}
	}

	public void Subscribe<TEvent>(IEventService.EventHandler eventHandler) where TEvent : IEvent
	{
		Subscribe(typeof(TEvent), eventHandler);
	}

	public void Subscribe(Type @event, IEventService.EventHandler eventHandler)
	{
		if (_events.ContainsKey(@event))
		{
			Dictionary<Type, IEventService.EventHandler> events = _events;
			events[@event] = (IEventService.EventHandler)Delegate.Combine(events[@event], eventHandler);
		}
		else
		{
			_events[@event] = eventHandler;
		}
	}

	public void SubscribeOnce<TEvent>(IEventService.EventHandler eventHandler) where TEvent : IEvent
	{
		SubscribeOnce(typeof(TEvent), eventHandler);
	}

	public void SubscribeOnce(Type @event, IEventService.EventHandler eventHandler)
	{
		if (_events.ContainsKey(@event))
		{
			Dictionary<Type, IEventService.EventHandler> events = _events;
			Type key = @event;
			events[key] = (IEventService.EventHandler)Delegate.Combine(events[key], new IEventService.EventHandler(Once));
		}
		else
		{
			_events[@event] = Once;
		}
		void Once(object? x)
		{
			Unsubscribe(@event, Once);
			eventHandler(x);
		}
	}

	public void Unsubscribe<TEvent>(IEventService.EventHandler eventHandler) where TEvent : IEvent
	{
		Unsubscribe(typeof(TEvent), eventHandler);
	}

	public void Unsubscribe(Type @event, IEventService.EventHandler eventHandler)
	{
		if (_events.ContainsKey(@event))
		{
			Dictionary<Type, IEventService.EventHandler> events = _events;
			events[@event] = (IEventService.EventHandler)Delegate.Remove(events[@event], eventHandler);
			if (_events[@event] == null)
			{
				_events.Remove(@event);
			}
		}
	}
}
