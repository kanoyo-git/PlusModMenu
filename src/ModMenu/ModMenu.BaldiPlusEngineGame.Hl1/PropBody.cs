using System.Collections.Generic;
using System.Linq;
using ModMenu.Hl1;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Hl1;

internal sealed class PropBody : MonoBehaviour
{
	public const float Gravity = Hl1Constants.Gravity;

	private const float GroundFriction = 6f;

	private const float Bounce = 0.3f;

	private const float HitSpeed = 8f;

	private const float MaxLifetime = 12f;

	private static readonly List<PropBody> _moving = new();

	private Solid _solid = null!;

	private Collider[] _colliders = new Collider[0];

	private Vector3 _centreOffset;

	private Vector3 _size;

	private Vector3 _velocity;

	private float _life;

	private readonly HashSet<Entity> _hit = new();

	public static IReadOnlyList<PropBody> Moving => _moving;

	public Solid Solid => _solid;

	public Vector3 Velocity => _velocity;

	public static event System.Action<Solid>? Settled;

	public static PropBody Launch(Solid solid, Collider[] colliders, Vector3 centreOffset, Vector3 size, Vector3 velocity)
	{
		var body = solid.GetComponent<PropBody>() ?? solid.gameObject.AddComponent<PropBody>();
		body._solid = solid;
		body._colliders = colliders;
		body._centreOffset = centreOffset;
		body._size = size;
		body._velocity = velocity;
		body._life = 0;
		body._hit.Clear();
		solid.Held = true;
		CullFollow.Attach(solid.gameObject);
		if (!_moving.Contains(body)) _moving.Add(body);
		return body;
	}

	public static void SettleAll()
	{
		foreach (var body in _moving.ToArray())
		{
			if (body != null) body.Settle();
		}
		_moving.Clear();
	}

	private void Update()
	{
		var dt = Time.deltaTime;
		if (dt <= 0f || _solid == null) return;
		_life += dt;
		var centre = transform.position + _centreOffset;
		var half = _size / 2;

		var flat = new Vector3(_velocity.x, 0, _velocity.z);
		var step = flat.magnitude * dt;
		if (step > 1e-4f)
		{
			var direction = flat / flat.magnitude;
			var box = new Vector3(centre.x, centre.y, centre.z);
			var travel = step;
			if (WallHit(box, half, direction, step, flat.magnitude, out var hit))
			{
				travel = Mathf.Max(0f, hit.distance - 0.05f);
				var into = Vector3.Dot(_velocity, hit.normal);
				if (into < 0) _velocity -= (1 + Bounce) * into * new Vector3(hit.normal.x, 0, hit.normal.z);
			}
			centre += direction * travel;
			HitEntities(centre, half, flat.magnitude, direction);
		}

		_velocity.y -= Gravity * dt;
		var bottom = centre.y - half.y + _velocity.y * dt;
		if (bottom + _size.y > Hl1Constants.Ceiling)
		{
			bottom = Hl1Constants.Ceiling - _size.y;
			if (_velocity.y > 0) _velocity.y = 0;
		}
		var support = Support(_solid, centre, _size, centre.y - half.y + 0.05f);
		var onGround = false;
		if (bottom <= support)
		{
			bottom = support;
			onGround = true;
			_velocity.y = _velocity.y < -6f ? -_velocity.y * 0.2f : 0f;
		}
		centre.y = bottom + half.y;
		if (onGround)
		{
			var speed = new Vector2(_velocity.x, _velocity.z).magnitude;
			var slowed = Mathf.Max(0f, speed - GroundFriction * Mathf.Max(speed, 3f) * dt);
			if (speed > 0) { _velocity.x *= slowed / speed; _velocity.z *= slowed / speed; }
		}
		transform.position = centre - _centreOffset;

		if ((onGround && _velocity.y == 0f && new Vector2(_velocity.x, _velocity.z).sqrMagnitude < 0.09f) || _life > MaxLifetime)
		{
			Settle();
		}
	}

	private void Settle()
	{
		_moving.Remove(this);
		if (_solid == null) { Destroy(this); return; }
		var centre = transform.position + _centreOffset;
		centre.y = Support(_solid, centre, _size, centre.y - _size.y / 2 + 0.05f) + _size.y / 2;
		KeepOffPlayer(ref centre);
		transform.position = centre - _centreOffset;
		foreach (var c in _colliders)
		{
			if (c != null) c.enabled = true;
		}
		_solid.Held = false;
		_solid.Refresh();
		LevelSolids.Add(_solid);
		PropBlocker.Attach(_solid);
		Settled?.Invoke(_solid);
		Destroy(this);
	}

	private void OnDestroy() => _moving.Remove(this);

	private void KeepOffPlayer(ref Vector3 centre)
	{
		var player = Singleton<CoreGameManager>.Instance?.GetPlayer(0);
		if (player == null) return;
		var radius = player.plm.cc.radius;
		var p = player.transform.position;
		var bottom = centre.y - _size.y / 2;
		var feet = Hl1Mode.Controller != null ? Hl1Mode.Controller.State.Feet : 0f;
		if (bottom >= feet + Hl1Constants.StandHeight || centre.y + _size.y / 2 <= feet + Hl1Constants.StepSize) return;
		var away = new Vector3(centre.x - p.x, 0, centre.z - p.z);
		if (away.sqrMagnitude < 1e-4f) away = player.transform.forward;
		away.Normalize();
		for (var i = 0; i < 40 && Mathf.Abs(centre.x - p.x) < _size.x / 2 + radius && Mathf.Abs(centre.z - p.z) < _size.z / 2 + radius; i++)
		{
			centre += away * 0.5f;
		}
	}

	private static bool WallHit(Vector3 centre, Vector3 half, Vector3 direction, float distance, float speed, out RaycastHit wall)
	{
		wall = default;
		var found = false;
		var nearest = float.MaxValue;
		foreach (var hit in Physics.BoxCastAll(centre, half * 0.9f, direction, Quaternion.identity, distance, LevelSolids.PlayerMask, QueryTriggerInteraction.Ignore))
		{
			if (!IsWall(hit.collider) || hit.distance >= nearest) continue;
			if (speed >= BreakSpeed && BreakWindow(hit.collider)) continue;
			if (hit.distance <= 0f && hit.point == Vector3.zero) continue;
			nearest = hit.distance;
			wall = hit;
			found = true;
		}
		return found;
	}

	public const float BreakSpeed = 6f;

	internal static bool BreakWindow(Collider c)
	{
		var window = c.GetComponentInParent<Window>();
		if (window == null) return false;
		window.Break(true);
		return !c.enabled;
	}

	internal static void DropWhatsOn(Solid support)
	{
		LooseFall.DropOver(support);
		foreach (var other in LevelSolids.All.Concat(LevelSolids.Decor).ToArray())
		{
			if (other == null || other == support || other.Held || !other.gameObject.activeInHierarchy) continue;
			if (Mathf.Abs(other.Bottom - support.Top) > 0.4f) continue;
			var cx = (other.MinX + other.MaxX) / 2;
			var cz = (other.MinZ + other.MaxZ) / 2;
			if (cx < support.MinX || cx > support.MaxX || cz < support.MinZ || cz > support.MaxZ) continue;
			Fall(other);
		}
	}

	private static void Fall(Solid solid)
	{
		solid.Refresh();
		var centre = new Vector3((solid.MinX + solid.MaxX) / 2, (solid.Bottom + solid.Top) / 2, (solid.MinZ + solid.MaxZ) / 2);
		var size = new Vector3(solid.MaxX - solid.MinX, solid.Top - solid.Bottom, solid.MaxZ - solid.MinZ);
		var colliders = solid.Colliders.Where(c => c != null && c.enabled).ToArray();
		foreach (var c in colliders) c.enabled = false;
		LevelSolids.Remove(solid);
		Launch(solid, colliders, centre - solid.transform.position, size, Vector3.zero);
	}

	private static bool IsWall(Collider c) =>
		c.GetComponentInParent<Solid>() == null && c.GetComponentInParent<PlayerManager>() == null && c.GetComponentInParent<Entity>() == null;

	private void HitEntities(Vector3 centre, Vector3 half, float speed, Vector3 direction)
	{
		if (speed < HitSpeed) return;
		foreach (var entity in Entity.allEntities)
		{
			if (entity == null || _hit.Contains(entity) || entity.GetComponent<PlayerManager>() != null) continue;
			var p = entity.transform.position;
			if (Mathf.Abs(p.x - centre.x) > half.x + 1.5f || Mathf.Abs(p.z - centre.z) > half.z + 1.5f || centre.y - half.y > 8f) continue;
			_hit.Add(entity);
			entity.AddForce(new Force(direction, speed * 0.8f, -speed * 1.2f));
		}
	}

	internal static float Support(Solid? self, Vector3 centre, Vector3 size, float below)
	{
		var support = 0f;
		var hx = size.x / 2;
		var hz = size.z / 2;
		foreach (var solid in LevelSolids.All)
		{
			if (solid == null || solid == self || solid.Held || !solid.gameObject.activeInHierarchy) continue;
			if (!(solid.MinX < centre.x + hx && solid.MaxX > centre.x - hx && solid.MinZ < centre.z + hz && solid.MaxZ > centre.z - hz)) continue;
			if (solid.Top <= below && solid.Top > support) support = solid.Top;
		}
		return support;
	}

	internal static void LiftOntoFurniture(Solid? self, ref Vector3 centre, Vector3 size)
	{
		var hx = size.x / 2;
		var hz = size.z / 2;
		var bottom = centre.y - size.y / 2;
		var lifted = bottom;
		foreach (var solid in LevelSolids.All)
		{
			if (solid == null || solid == self || solid.Held || !solid.gameObject.activeInHierarchy) continue;
			if (!(solid.MinX < centre.x + hx && solid.MaxX > centre.x - hx && solid.MinZ < centre.z + hz && solid.MaxZ > centre.z - hz)) continue;
			if (solid.Top > bottom && solid.Top - bottom < size.y / 2 && solid.Top > lifted) lifted = solid.Top;
		}
		centre.y += lifted - bottom;
	}

	private static BoxCollider? _probe;

	internal static bool Unwall(ref Vector3 centre, Vector3 size, Vector3 safe)
	{
		if (_probe == null)
		{
			var go = new GameObject("ModMenuPropProbe") { layer = 2 };
			DontDestroyOnLoad(go);
			go.transform.position = new Vector3(0, -1000, 0);
			_probe = go.AddComponent<BoxCollider>();
			_probe.isTrigger = true;
		}
		_probe.size = size * 0.9f;
		for (var i = 0; i < 8; i++)
		{
			var walls = Physics.OverlapBox(centre, size * 0.45f, Quaternion.identity, LevelSolids.PlayerMask, QueryTriggerInteraction.Ignore).Where(IsWall).ToList();
			if (walls.Count == 0) return true;
			foreach (var wall in walls)
			{
				if (Physics.ComputePenetration(_probe, centre, Quaternion.identity, wall, wall.transform.position, wall.transform.rotation, out var direction, out var distance))
				{
					centre += new Vector3(direction.x, 0, direction.z) * (distance + 0.05f);
				}
			}
		}
		var from = centre;
		for (var t = 0.1f; t <= 1f; t += 0.1f)
		{
			var tried = Vector3.Lerp(from, new Vector3(safe.x, from.y, safe.z), t);
			if (!Physics.OverlapBox(tried, size * 0.45f, Quaternion.identity, LevelSolids.PlayerMask, QueryTriggerInteraction.Ignore).Any(IsWall))
			{
				centre = tried;
				return true;
			}
		}
		return false;
	}
}
