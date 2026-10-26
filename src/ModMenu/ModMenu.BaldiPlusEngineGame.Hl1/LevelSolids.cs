using System.Collections.Generic;
using System.Linq;
using ModMenu.Hl1;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Hl1;

internal sealed class Solid : MonoBehaviour
{
	public float MinX, MaxX, MinZ, MaxZ, Bottom, Top;

	public Collider[] Colliders = new Collider[0];

	public bool Held;

	public bool HeightFromColliders;

	public bool Shallow;

	public bool Decor;

	public void Refresh()
	{
		Physics.SyncTransforms();
		Colliders = (Shallow ? GetComponents<Collider>() : GetComponentsInChildren<Collider>(true)).Where(LevelSolids.Blocks).ToArray();
		var renderers = Shallow ? GetComponents<Renderer>() : GetComponentsInChildren<Renderer>(true);
		var footprint = new Bounds(transform.position, Vector3.zero);
		if (Colliders.Length > 0)
		{
			footprint = Colliders[0].bounds;
			foreach (var c in Colliders) footprint.Encapsulate(c.bounds);
		}
		var height = footprint;
		if (renderers.Length > 0 && !HeightFromColliders)
		{
			height = renderers[0].bounds;
			foreach (var r in renderers) height.Encapsulate(r.bounds);
		}
		if (Decor) footprint = height;
		MinX = footprint.min.x;
		MaxX = footprint.max.x;
		MinZ = footprint.min.z;
		MaxZ = footprint.max.z;
		Bottom = height.min.y;
		Top = height.max.y;
	}

	public bool Overlaps(Vector3 position, float halfWidth) =>
		MinX < position.x + halfWidth && MaxX > position.x - halfWidth && MinZ < position.z + halfWidth && MaxZ > position.z - halfWidth;
}

internal static class LevelSolids
{
	private static readonly List<Solid> _solids = new();

	private static readonly List<Solid> _decor = new();

	private static int _playerLayer;

	public static IReadOnlyList<Solid> All => _solids;

	public static IReadOnlyList<Solid> Decor => _decor;

	public static bool Built { get; private set; }

	public static int PlayerMask
	{
		get
		{
			var mask = 0;
			for (var i = 0; i < 32; i++)
			{
				if (!Physics.GetIgnoreLayerCollision(_playerLayer, i)) mask |= 1 << i;
			}
			return mask;
		}
	}

	internal static bool Blocks(Collider c) => !c.isTrigger && !Physics.GetIgnoreLayerCollision(_playerLayer, c.gameObject.layer);

	public static void Build(EnvironmentController ec)
	{
		Clear();
		var player = Singleton<CoreGameManager>.Instance?.GetPlayer(0);
		_playerLayer = player != null ? player.gameObject.layer : 0;
		foreach (var room in ec.rooms)
		{
			if (room.objectObject == null) continue;
			foreach (Transform child in room.objectObject.transform)
			{
				AddSolids(child);
			}
			foreach (var renderer in room.GetComponentsInChildren<Renderer>(true))
			{
				var t = renderer.transform;
				if (!t.name.StartsWith("Decor_") || t.name.Contains("ExitSign") || t.GetComponentInParent<Solid>() is Solid { Decor: false }) continue;
				if (t.GetComponentsInChildren<Collider>(true).Any(Blocks) || renderer.bounds.min.y > 7f) continue;
				var decor = t.GetComponent<Solid>() ?? t.gameObject.AddComponent<Solid>();
				decor.Decor = true;
				decor.Refresh();
				if (!_decor.Contains(decor)) _decor.Add(decor);
			}
		}
		Built = true;
	}

	private static void AddSolids(Transform t)
	{
		var colliders = t.GetComponentsInChildren<Collider>(true).Where(Blocks).ToArray();
		if (colliders.Length == 0) return;
		var union = colliders[0].bounds;
		var area = 0f;
		foreach (var c in colliders)
		{
			union.Encapsulate(c.bounds);
			area += c.bounds.size.x * c.bounds.size.z;
		}
		var compound = colliders.Length > 1 && t.childCount > 0 && union.size.x * union.size.z > 2f * area + 4f;
		var solid = t.GetComponent<Solid>();
		if (compound)
		{
			if (t.GetComponents<Collider>().Any(Blocks))
			{
				solid ??= t.gameObject.AddComponent<Solid>();
				solid.Shallow = true;
				solid.Refresh();
				_solids.Add(solid);
			}
			else if (solid != null)
			{
				Object.DestroyImmediate(solid);
			}
			foreach (Transform child in t) AddSolids(child);
			return;
		}
		solid ??= t.gameObject.AddComponent<Solid>();
		solid.Refresh();
		_solids.Add(solid);
	}

	public static void Add(Solid solid)
	{
		if (solid.Decor) return;
		if (!_solids.Contains(solid)) _solids.Add(solid);
	}

	public static void Remove(Solid solid) => _solids.Remove(solid);

	public static void Clear()
	{
		_solids.Clear();
		_decor.Clear();
		PropBlocker.Clear();
		Built = false;
	}

	public static IEnumerable<Solid> Near(Vector3 position, float halfWidth)
	{
		for (var i = _solids.Count - 1; i >= 0; i--)
		{
			var solid = _solids[i];
			if (solid == null)
			{
				_solids.RemoveAt(i);
				continue;
			}
			if (!solid.Held && solid.gameObject.activeInHierarchy && solid.Overlaps(position, halfWidth)) yield return solid;
		}
	}
}

internal sealed class UnityWorld : IHl1World
{
	public Vector3 Position;

	public float HalfWidth = 2f;

	public float? GroundUnder(float feet, float hullHeight) => GroundAt(Position, feet);

	private float GroundAt(Vector3 position, float feet)
	{
		var ground = 0f;
		foreach (var solid in LevelSolids.Near(position, HalfWidth))
		{
			if (solid.Top <= feet + 0.001f && solid.Top > ground) ground = solid.Top;
		}
		return ground;
	}

	public float CeilingAbove(float feet, float hullHeight)
	{
		var ceiling = feet >= Hl1Constants.Ceiling ? float.PositiveInfinity : Hl1Constants.Ceiling;
		foreach (var solid in LevelSolids.Near(Position, HalfWidth))
		{
			if (solid.Bottom >= feet + 0.01f && solid.Bottom < ceiling) ceiling = solid.Bottom;
		}
		return ceiling;
	}

	public bool HullClear(float feet, float hullHeight)
	{
		if (feet >= Hl1Constants.Ceiling) return true;
		if (feet + hullHeight > Hl1Constants.Ceiling) return false;
		foreach (var solid in LevelSolids.Near(Position, HalfWidth))
		{
			if (solid.Bottom < feet + hullHeight && solid.Top > feet + 0.001f) return false;
		}
		return true;
	}

	public float StepUp(float feet, float hullHeight)
	{
		var up = feet;
		foreach (var solid in LevelSolids.Near(Position, HalfWidth))
		{
			if (solid.Top > up + 0.001f && solid.Top <= feet + Hl1Constants.StepSize && HullClear(solid.Top, hullHeight)) up = solid.Top;
		}
		return up;
	}

	public bool EdgeAhead(float feet, Hl1Vec flatDirection)
	{
		if (feet <= 0.001f) return false;
		var ahead = Position + new Vector3(flatDirection.X, 0, flatDirection.Z) * Hl1Constants.EdgeProbeAhead;
		return GroundAt(ahead, feet) < feet - Hl1Constants.EdgeProbeDown;
	}
}
