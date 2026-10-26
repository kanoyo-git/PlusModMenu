using UnityEngine;

namespace PlusModMenu.Core.Unity.Extensions;

public static class GameObjectEx
{
	public static void Toggle(this GameObject @this)
	{
		@this.SetActive(!@this.activeSelf);
	}

	public static TComponent GetOrAddComponent<TComponent>(this GameObject @this) where TComponent : Component
	{
		TComponent component = @this.GetComponent<TComponent>();
		if (!(component != null))
		{
			return @this.gameObject.AddComponent<TComponent>();
		}
		return component;
	}
}
