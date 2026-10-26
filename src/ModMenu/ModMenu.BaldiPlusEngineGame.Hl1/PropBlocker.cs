using System.Collections.Generic;
using HarmonyLib;
using ModMenu.Hl1;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Hl1;

internal static class PropBlocker
{
	private static readonly List<Solid> _moved = new();

	private static readonly AccessTools.FieldRef<Entity, float> _radius = AccessTools.FieldRefAccess<Entity, float>("colliderRadius");

	public static IReadOnlyList<Solid> Moved => _moved;

	public static void Attach(Solid solid)
	{
		if (solid == null || solid.Decor || solid.HeightFromColliders || solid.GetComponentInParent<Pickup>() != null) return;
		if (!_moved.Contains(solid)) _moved.Add(solid);
	}

	public static void Keep(Transform body, float radius, Vector3 start)
	{
		if (_moved.Count == 0) return;
		var position = body.position;
		var moved = false;
		for (var i = _moved.Count - 1; i >= 0; i--)
		{
			var solid = _moved[i];
			if (solid == null)
			{
				_moved.RemoveAt(i);
				continue;
			}
			if (solid.Held || !solid.gameObject.activeInHierarchy) continue;
			if (BoxPush.Step(start.x, start.z, position.x, position.z, radius, solid.MinX, solid.MaxX, solid.MinZ, solid.MaxZ) is (float x, float z))
			{
				position.x = x;
				position.z = z;
				moved = true;
			}
		}
		if (moved) body.position = position;
	}

	public static void Clear() => _moved.Clear();

	private static float Radius(Navigator navigator) => navigator.Entity != null ? _radius(navigator.Entity) : 1f;

	[HarmonyPatch(typeof(Navigator), nameof(Navigator.MoveTowardsDestination))]
	private class MoveTowardsDestination
	{
		private static void Prefix(Navigator __instance, out Vector3 __state) => __state = __instance.transform.position;

		private static void Postfix(Navigator __instance, Vector3 __state) => Keep(__instance.transform, Radius(__instance), __state);
	}

	[HarmonyPatch(typeof(Entity), nameof(Entity.MoveWithCollision))]
	private class MoveWithCollision
	{
		private static void Prefix(Entity __instance, out Vector3 __state) => __state = __instance.transform.position;

		private static void Postfix(Entity __instance, Vector3 __state)
		{
			if (__instance is PlayerEntity) return;
			Keep(__instance.transform, _radius(__instance), __state);
		}
	}
}
