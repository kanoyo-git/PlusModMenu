using System.Collections.Generic;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Hl1;
using ModMenu.Hl1;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

[HarmonyPatch(typeof(PlayerEntity), nameof(PlayerEntity.MoveWithCollision))]
internal class Hl1CollisionPatch
{
	private static readonly HashSet<Collider> _ignored = new();

	private static CharacterController? _controller;

	internal static float? Feet { get; set; }

	internal static bool Grounded { get; set; } = true;

	private static void Prefix(PlayerEntity __instance, Vector3 movement)
	{
		if (Feet is not float feet)
		{
			return;
		}
		if (_controller == null || _controller.gameObject != __instance.gameObject)
		{
			RestoreAll();
			_controller = __instance.GetComponent<CharacterController>();
		}
		var reach = _controller.radius + movement.magnitude + 1f;
		foreach (var solid in LevelSolids.All)
		{
			if (solid == null || !solid.Overlaps(__instance.transform.position, reach)) continue;
			var ignore = solid.Held || !solid.gameObject.activeInHierarchy || solid.Top <= feet + (Grounded ? Hl1Constants.StepSize : 0.01f);
			foreach (var collider in solid.Colliders)
			{
				if (collider == null || _ignored.Contains(collider) == ignore) continue;
				Physics.IgnoreCollision(_controller, collider, ignore);
				if (ignore) _ignored.Add(collider);
				else _ignored.Remove(collider);
			}
		}
	}

	internal static void RestoreAll()
	{
		if (_controller != null)
		{
			foreach (var collider in _ignored)
			{
				if (collider != null) Physics.IgnoreCollision(_controller, collider, false);
			}
		}
		_ignored.Clear();
		Feet = null;
		Grounded = true;
	}
}
