using System;
using System.Linq;
using UnityEngine;

namespace PlusModMenu.Core.Unity.Extensions;

public static class ComponentEx
{
	public static TComponent GetOrAddComponent<TComponent>(this Component @this) where TComponent : Component
	{
		TComponent component = @this.GetComponent<TComponent>();
		if (!(component != null))
		{
			return @this.gameObject.AddComponent<TComponent>();
		}
		return component;
	}

	public static TComponent? GetComponentInChildren<TComponent>(this Component @this, Func<TComponent, bool> predicate) where TComponent : Component
	{
		return @this.GetComponentsInChildren<TComponent>().FirstOrDefault(predicate);
	}
}
