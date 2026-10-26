using System;
using System.Collections.Generic;
using System.Linq;
using PlusModMenu.Core.Common.Extensions;
using PlusModMenu.Api.Interfaces;

namespace PlusModMenu.Api;

public class Service
{
	private static readonly Dictionary<Type, IService> _services = new Dictionary<Type, IService>();

	public static TService Register<TService>(TService serviceInstance) where TService : IService
	{
		_services.Add(typeof(TService), serviceInstance);
		return serviceInstance;
	}

	public static TService Get<TService>() where TService : IService
	{
		Type serviceType = typeof(TService);
		if (_services.TryGetValue(serviceType, out IService value))
		{
			return (TService)value;
		}
		value = _services.FirstOrDefault((KeyValuePair<Type, IService> x) => x.Key.IsAssignableFrom(serviceType)).Value;
		if (value is TService)
		{
			return (TService)value;
		}
		throw new KeyNotFoundException("'" + serviceType.Name + "' is not implemented in a service that is currently registered.");
	}

	public static bool IsRegistered<TService>() where TService : IService
	{
		Type serviceType = typeof(TService);
		if (_services.ContainsKey(serviceType))
		{
			return true;
		}
		return !_services.FirstOrDefault((KeyValuePair<Type, IService> x) => x.Key.IsAssignableFrom(serviceType)).IsDefault();
	}
}
