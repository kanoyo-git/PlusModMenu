using System;

namespace PlusModMenu.Api.Interfaces.Services;

public interface IEventService : IService
{
	public delegate void EventHandler(object? args);

	void Publish<TEvent>(object? args) where TEvent : IEvent;

	void Subscribe<TEvent>(EventHandler eventHandler) where TEvent : IEvent;

	void Subscribe(Type @event, EventHandler eventHandler);

	void SubscribeOnce<TEvent>(EventHandler eventHandler) where TEvent : IEvent;

	void SubscribeOnce(Type @event, EventHandler eventHandler);

	void Unsubscribe<TEvent>(EventHandler eventHandler) where TEvent : IEvent;

	void Unsubscribe(Type @event, EventHandler eventHandler);
}
