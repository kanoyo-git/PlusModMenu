using System.Collections.Generic;
using System.Linq;
using PlusModMenu.Core.Unity;
using ModMenu.Hl1;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Hl1;

internal sealed class CarryController : MonoBehaviour
{
	public const float CrateSize = 1.5f;

	private static readonly List<CarryController> _all = new();

	private Hl1Controller? _controller;

	private PlayerManager _player = null!;

	private Solid? _held;

	private Pickup? _heldPickup;

	private Collider[] _disabled = new Collider[0];

	private Vector3 _centreOffset;

	private Vector3 _size;

	private Vector3 _pickedFrom;

	private bool _unlimited;

	private float _free = 1f;

	private float _yesClipCooldown;

	private float _yesClipPressure;

	private float _yesClipNeed = 0.8f;

	private Vector3 _baseOffset;

	private Vector3 _baseHalf;

	private Quaternion _baseRotation = Quaternion.identity;

	private Quaternion _turn = Quaternion.identity;

	private Quaternion? _frozenYaw;

	private Quaternion _frozenPitch;

	internal static int YesClips { get; private set; }

	public const float GravityGunMaxSize = 32f;

	public const float YesClipReach = 8f;

	private static readonly Dictionary<Pickup, float> _crated = new();

	public Transform? Held => _held != null ? _held.transform : null;

	public Vector3 HeldCentre => _held != null ? _held.transform.position + _centreOffset : Vector3.zero;

	public bool HoldingWithGravityGun => _held != null && _unlimited;

	public static CarryController Attach(PlayerManager player, Hl1Controller? controller)
	{
		var carry = player.GetComponent<CarryController>() ?? player.gameObject.AddComponent<CarryController>();
		carry._player = player;
		carry._controller = controller;
		if (!_all.Contains(carry)) _all.Add(carry);
		return carry;
	}

	internal static int CrateCount => _crated.Count(pair => pair.Key != null && pair.Key.GetComponent<Solid>() != null);

	internal static void RemoveCrates()
	{
		foreach (var pair in _crated)
		{
			var pickup = pair.Key;
			if (pickup == null) continue;
			if (pickup.GetComponent<Solid>() is Solid solid)
			{
				LevelSolids.Remove(solid);
				DestroyImmediate(solid);
			}
			var box = pickup.transform.Find("Hl1Crate");
			if (box != null) DestroyImmediate(box.gameObject);
			var position = pickup.transform.position;
			position.y = pair.Value;
			pickup.transform.position = position;
		}
		_crated.Clear();
	}

	internal static void DropAll()
	{
		foreach (var carry in _all.ToArray())
		{
			if (carry != null) carry.Drop();
		}
		PropBody.SettleAll();
	}

	public void Detach()
	{
		Drop();
		_all.Remove(this);
		DestroyImmediate(this);
	}

	private void OnDestroy() => _all.Remove(this);

	private void Update()
	{
		if (_player == null || Time.timeScale == 0) return;
		if (GameControls)
		{
			Drop();
			return;
		}
		if (_yesClipCooldown > 0) _yesClipCooldown -= Time.deltaTime;
		if (Input.GetKeyDown(Config.Instance.Hl1UseKey) && (_controller != null || HoldingWithGravityGun))
		{
			Use();
		}
		Turn();
		YesClip();
	}

	private bool GameControls => _controller != null ? _controller.GameControls : _player.plm.Entity.InteractionDisabled;

	private float Feet => _controller != null ? _controller.State.Feet : 0f;

	private float HullHeight => _controller != null ? _controller.State.HullHeight : Hl1Constants.StandHeight;

	private float HalfWidth => _controller != null ? _controller.World.HalfWidth : _player.plm.cc.radius;

	public bool Use() => _held != null ? Place() : TryPick();

	public bool UseGravityGun()
	{
		if (_held != null) return Throw();
		if (!LevelSolids.Built && Singleton<BaseGameManager>.Instance?.Ec is EnvironmentController ec) LevelSolids.Build(ec);
		var target = Aimed(GravityGunReach);
		if (target == null) return false;
		_unlimited = true;
		if (TryPick(target)) return true;
		_unlimited = false;
		return false;
	}

	public const float GravityGunReach = 30f;

	private void YesClip()
	{
		var dt = Time.deltaTime;
		if (_yesClipCooldown > 0 || !Wedged(out var forward, out var wall))
		{
			_yesClipPressure = Mathf.Max(0f, _yesClipPressure - dt * 0.5f);
			return;
		}
		var airborne = _controller != null && !_controller.State.OnGround;
		_yesClipPressure += dt * (airborne ? 1f : 0.35f);
		if (_yesClipPressure < _yesClipNeed) return;
		if (Behind(_player.transform.position, forward, wall, HalfWidth) is not Vector3 target) return;
		_yesClipPressure = 0f;
		_yesClipNeed = Random.Range(0.6f, 1.3f);
		_yesClipCooldown = 0.5f;
		YesClips++;
		_player.Teleport(target);
		if (_held != null) Hold();
	}

	private bool Wedged(out Vector3 forward, out float wallDistance)
	{
		wallDistance = 0;
		forward = View().forward;
		forward.y = 0;
		if (forward.sqrMagnitude < 0.01f || !PushingForward()) return false;
		forward.Normalize();
		var origin = _player.transform.position;
		var radius = HalfWidth;
		if (_held != null)
		{
			if (_free > 0.6f || Wall(origin, forward, radius + _size.magnitude + 2f) is not float held) return false;
			wallDistance = held;
			return true;
		}
		foreach (var hit in Physics.RaycastAll(origin, forward, radius + 2f, LevelSolids.PlayerMask, QueryTriggerInteraction.Ignore).OrderBy(h => h.distance))
		{
			if (hit.collider.GetComponentInParent<PlayerManager>() != null) continue;
			var prop = hit.collider.GetComponentInParent<Solid>();
			if (prop == null || prop.Held || prop.Top <= Feet + Hl1Constants.StepSize || PartOfTheGame(prop)) return false;
			var depth = Mathf.Abs(forward.x) * (prop.MaxX - prop.MinX) + Mathf.Abs(forward.z) * (prop.MaxZ - prop.MinZ);
			if (Wall(origin, forward, hit.distance + depth + 1f) is not float behind || behind - hit.distance > depth + 0.75f) return false;
			wallDistance = behind;
			return true;
		}
		return false;
	}

	private bool PushingForward()
	{
		if (_controller != null) return _controller.LastInput.Forward >= 0.5f;
		Singleton<InputManager>.Instance.GetAnalogInput(_player.plm.movementAnalogData, out var analog, out _, 0.1f);
		return analog.y >= 0.5f;
	}

	public bool Turning => _turnLock;

	private bool _turnLock;

	private const float DoubleClick = 0.35f;

	private float _lastWheelClick = float.NegativeInfinity;

	private void Turn()
	{
		if (_held == null)
		{
			StopTurning();
			return;
		}
		if (Input.GetKeyDown(KeyCode.Mouse2))
		{
			if (_turnLock) StopTurning();
			else StartTurning();
			if (Time.unscaledTime - _lastWheelClick < DoubleClick)
			{
				Straighten();
				_lastWheelClick = float.NegativeInfinity;
			}
			else
			{
				_lastWheelClick = Time.unscaledTime;
			}
		}
		if (!_turnLock) return;
		Singleton<InputManager>.Instance.GetAnalogInput(_player.plm.cameraAnalogData, out _, out var delta, 0.1f);
		var sensitivity = Singleton<PlayerFileManager>.Instance.mouseCameraSensitivity;
		TurnBy(-delta.x * sensitivity, delta.y * sensitivity);
		KeepView();
	}

	internal void StartTurning()
	{
		if (_turnLock || _held == null) return;
		_turnLock = true;
		_frozenYaw = _player.transform.rotation;
		_frozenPitch = View().parent.localRotation;
	}

	internal void StopTurning()
	{
		if (!_turnLock) return;
		_turnLock = false;
		_frozenYaw = null;
	}

	public static bool AnyTurning => _all.Any(x => x != null && x._turnLock);

	internal void TurnBy(float yaw, float pitch)
	{
		if (_held == null || (yaw == 0f && pitch == 0f)) return;
		_turn = Quaternion.AngleAxis(yaw, Vector3.up) * Quaternion.AngleAxis(pitch, View().right) * _turn;
		ApplyTurn();
		Hold();
	}

	internal void Straighten()
	{
		if (_held == null) return;
		var forward = _turn * Vector3.forward;
		forward.y = 0f;
		if (forward.sqrMagnitude < 1e-4f)
		{
			forward = _turn * Vector3.up;
			forward.y = 0f;
		}
		var yaw = forward.sqrMagnitude < 1e-4f ? 0f : Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
		_turn = Quaternion.AngleAxis(Mathf.Round(yaw / 90f) * 90f, Vector3.up);
		ApplyTurn();
		Hold();
	}

	internal float HeldTilt => _held == null ? 0f : Vector3.Angle(_turn * Vector3.up, Vector3.up);

	internal bool GravityGunCanLift(Solid solid)
	{
		var unlimited = _unlimited;
		_unlimited = true;
		var can = CanCarry(solid);
		_unlimited = unlimited;
		return can;
	}

	private void KeepView()
	{
		if (_frozenYaw is not Quaternion yaw) return;
		_player.transform.rotation = yaw;
		View().parent.localRotation = _frozenPitch;
	}

	private void ApplyTurn()
	{
		_held!.transform.rotation = _turn * _baseRotation;
		_centreOffset = _turn * _baseOffset;
		var m = Matrix4x4.Rotate(_turn);
		var h = _baseHalf;
		_size = 2 * new Vector3(
			Mathf.Abs(m.m00) * h.x + Mathf.Abs(m.m01) * h.y + Mathf.Abs(m.m02) * h.z,
			Mathf.Abs(m.m10) * h.x + Mathf.Abs(m.m11) * h.y + Mathf.Abs(m.m12) * h.z,
			Mathf.Abs(m.m20) * h.x + Mathf.Abs(m.m21) * h.y + Mathf.Abs(m.m22) * h.z);
	}

	private static float? Wall(Vector3 origin, Vector3 direction, float reach)
	{
		foreach (var hit in Physics.RaycastAll(origin, direction, reach, LevelSolids.PlayerMask, QueryTriggerInteraction.Ignore).OrderBy(h => h.distance))
		{
			if (hit.collider.GetComponentInParent<PlayerManager>() != null || hit.collider.GetComponentInParent<Solid>() != null) continue;
			return hit.distance;
		}
		return null;
	}

	private Vector3? Behind(Vector3 origin, Vector3 direction, float wallDistance, float radius)
	{
		var ec = Singleton<BaseGameManager>.Instance?.Ec;
		var cc = _player.plm.cc;
		var half = Mathf.Max(0f, cc.height / 2 - cc.radius);
		for (var d = Mathf.Max(wallDistance + radius + 0.1f, 2 * radius + 0.1f); d <= wallDistance + YesClipReach; d += 0.25f)
		{
			var spot = origin + direction * d;
			var cell = ec != null ? ec.CellFromPosition(spot) : null;
			if (cell == null || cell.Null) continue;
			var centre = spot + cc.center;
			if (Physics.CheckCapsule(centre - Vector3.up * half, centre + Vector3.up * half, cc.radius * 0.95f, LevelSolids.PlayerMask, QueryTriggerInteraction.Ignore)) continue;
			return spot;
		}
		return null;
	}

	private void LateUpdate()
	{
		if (_held != null) Hold();
		KeepView();
	}

	public bool TryPick()
	{
		var target = Aimed();
		return target != null && TryPick(target);
	}

	public bool TryPick(GameObject target)
	{
		if (_held != null) return false;
		var pickup = target.GetComponentInParent<Pickup>();
		if (pickup != null)
		{
			if (!CanCarry(pickup)) return false;
			Take(Crate(pickup), pickup);
			return true;
		}
		var solid = target.GetComponent<Solid>() is Solid { Decor: true } decor ? decor : target.GetComponentInParent<Solid>();
		if (solid == null || !CanCarry(solid)) return false;
		Take(solid, null);
		return true;
	}

	public bool CanCarry(Solid solid)
	{
		if (solid.GetComponent<Pickup>() != null) return CanCarry(solid.GetComponent<Pickup>());
		if (solid.Decor) return true;
		var standingOn = Mathf.Abs(Feet - solid.Top) < 0.05f && solid.Overlaps(_player.transform.position, HalfWidth);
		if (_unlimited) return !standingOn && Mathf.Max(solid.MaxX - solid.MinX, solid.MaxZ - solid.MinZ) <= GravityGunMaxSize && !PartOfTheGame(solid);
		var gameScripts = solid.GetComponentsInChildren<MonoBehaviour>(true)
			.Any(m => m != null && m is not Solid && !CarryRules.GameScriptsAllowed.Contains(m.GetType().Name));
		return CarryRules.CanCarry(solid.MaxX - solid.MinX, solid.MaxZ - solid.MinZ, solid.Top - solid.Bottom, gameScripts, standingOn);
	}

	private static bool PartOfTheGame(Solid solid) =>
		solid.GetComponentInParent<Activity>() != null || solid.GetComponentInChildren<Activity>(true) != null
		|| solid.GetComponentInChildren<MathMachineNumber>(true) != null || solid.GetComponentInChildren<BalloonBusterBalloon>(true) != null
		|| solid.GetComponentInChildren<MatchActivityBalloon>(true) != null || solid.GetComponentInChildren<Notebook>(true) != null
		|| solid.GetComponentInChildren<Door>(true) != null || solid.GetComponentInParent<Elevator>() != null;

	private static bool CanCarry(Pickup pickup) => pickup.gameObject.activeInHierarchy && pickup.free && pickup.item != null;

	public bool Place()
	{
		if (_held == null) return false;
		Release(Vector3.zero);
		return true;
	}

	public bool Throw()
	{
		if (_held == null) return false;
		Release(View().forward * ThrowSpeed(_size));
		return true;
	}

	public static float ThrowSpeed(Vector3 size) => Mathf.Lerp(45f, 18f, Mathf.InverseLerp(2f, 20f, size.magnitude));

	private void Drop()
	{
		if (_held != null) Release(Vector3.zero);
	}

	private void Take(Solid solid, Pickup? pickup)
	{
		solid.Refresh();
		_held = solid;
		_heldPickup = pickup;
		_pickedFrom = solid.transform.position;
		solid.Held = true;
		CullFollow.Attach(solid.gameObject);
		var centre = new Vector3((solid.MinX + solid.MaxX) / 2, (solid.Bottom + solid.Top) / 2, (solid.MinZ + solid.MaxZ) / 2);
		_centreOffset = centre - solid.transform.position;
		_size = new Vector3(solid.MaxX - solid.MinX, solid.Top - solid.Bottom, solid.MaxZ - solid.MinZ);
		_baseOffset = _centreOffset;
		_baseHalf = _size / 2;
		_baseRotation = solid.transform.rotation;
		_turn = Quaternion.identity;
		var colliders = new List<Collider>(solid.Colliders);
		if (pickup != null) colliders.AddRange(pickup.GetComponents<Collider>());
		_disabled = colliders.Where(c => c != null && c.enabled).Distinct().ToArray();
		foreach (var c in _disabled) c.enabled = false;
		PropBody.DropWhatsOn(solid);
		Hold();
	}

	private void Release(Vector3 velocity)
	{
		var solid = _held!;
		var centre = solid.transform.position + _centreOffset;
		var eye = View().position;
		if (!PropBody.Unwall(ref centre, _size, new Vector3(eye.x, centre.y, eye.z)))
		{
			_turn = Quaternion.identity;
			ApplyTurn();
			centre = _pickedFrom + _centreOffset;
			velocity = Vector3.zero;
		}
		PropBody.LiftOntoFurniture(solid, ref centre, _size);
		solid.transform.position = centre - _centreOffset;
		var colliders = _disabled;
		_disabled = new Collider[0];
		_held = null;
		_heldPickup = null;
		_unlimited = false;
		StopTurning();
		LevelSolids.Remove(solid);
		PropBody.Launch(solid, colliders, _centreOffset, _size, velocity);
	}

	private void Hold()
	{
		var camera = View();
		var eye = camera.position;
		var forward = camera.forward;
		var extent = Mathf.Abs(forward.x) * _size.x / 2 + Mathf.Abs(forward.y) * _size.y / 2 + Mathf.Abs(forward.z) * _size.z / 2;
		var full = CarryRules.HoldDistance(extent, 1f);
		var free = 1f;
		foreach (var hit in Physics.RaycastAll(eye, forward, full + extent, ~0, QueryTriggerInteraction.Ignore).OrderBy(h => h.distance))
		{
			if (hit.collider.GetComponentInParent<PlayerManager>() != null) continue;
			if (hit.collider.GetComponentInParent<Solid>() is Solid solid && hit.point.y > solid.Top) continue;
			if (hit.distance - extent < full && PushingForward() && PropBody.BreakWindow(hit.collider)) continue;
			free = Mathf.Clamp01((hit.distance - extent) / full);
			break;
		}
		_free = free;
		var centre = eye + forward * CarryRules.HoldDistance(extent, free);
		centre.y = Mathf.Clamp(centre.y, _size.y / 2, Mathf.Max(_size.y / 2, Hl1Constants.Ceiling - _size.y / 2));
		_held!.transform.position = centre - _centreOffset;
	}

	private float? Support(Vector3 centre, float heldTop)
	{
		var support = 0f;
		var half = new Vector2(_size.x / 2, _size.z / 2);
		foreach (var solid in LevelSolids.All)
		{
			if (solid == null || solid == _held || solid.Held || !solid.gameObject.activeInHierarchy) continue;
			var overlaps = solid.MinX < centre.x + half.x && solid.MaxX > centre.x - half.x && solid.MinZ < centre.z + half.y && solid.MaxZ > centre.z - half.y;
			if (!overlaps) continue;
			if (solid.Top > heldTop + 0.3f) return null;
			if (solid.Top > support) support = solid.Top;
		}
		return support;
	}

	private GameObject? Aimed(float? reachOverride = null)
	{
		var camera = View();
		var reach = reachOverride ?? (_player.pc != null ? _player.pc.Reach : 10f);
		GameObject? found = null;
		var distance = reach;
		foreach (var hit in Physics.RaycastAll(camera.position, camera.forward, reach, ~0, QueryTriggerInteraction.Collide).OrderBy(h => h.distance))
		{
			if (hit.collider.GetComponentInParent<PlayerManager>() != null) continue;
			if (hit.collider.GetComponentInParent<Pickup>() != null)
			{
				found = hit.collider.gameObject;
				distance = hit.distance;
				break;
			}
			var solid = hit.collider.GetComponentInParent<Solid>();
			if (solid != null && LevelSolids.All.Contains(solid))
			{
				if (hit.point.y > solid.Top + 0.5f) continue;
				found = hit.collider.gameObject;
				distance = hit.distance;
				break;
			}
			if (!hit.collider.isTrigger)
			{
				distance = hit.distance;
				break;
			}
		}
		var ray = new Ray(camera.position, camera.forward);
		foreach (var decor in LevelSolids.Decor)
		{
			if (decor == null || decor.Held || !decor.gameObject.activeInHierarchy) continue;
			var box = new Bounds(new Vector3((decor.MinX + decor.MaxX) / 2, (decor.Bottom + decor.Top) / 2, (decor.MinZ + decor.MaxZ) / 2),
				new Vector3(Mathf.Max(decor.MaxX - decor.MinX, 1f), decor.Top - decor.Bottom, Mathf.Max(decor.MaxZ - decor.MinZ, 1f)));
			if (box.IntersectRay(ray, out var d) && d < distance)
			{
				found = decor.gameObject;
				distance = d;
			}
		}
		return found;
	}

	private Transform View() => Singleton<CoreGameManager>.Instance.GetCamera(_player.playerNumber).camCom.transform;

	private static Solid Crate(Pickup pickup)
	{
		var solid = pickup.GetComponent<Solid>();
		if (solid != null) return solid;
		var box = new GameObject("Hl1Crate");
		box.layer = 2;
		box.transform.SetParent(pickup.transform, false);
		var collider = box.AddComponent<BoxCollider>();
		collider.size = Vector3.one * CrateSize;
		var position = pickup.transform.position;
		_crated[pickup] = position.y;
		position.y = CrateSize / 2;
		pickup.transform.position = position;
		solid = pickup.gameObject.AddComponent<Solid>();
		solid.HeightFromColliders = true;
		solid.Refresh();
		LevelSolids.Add(solid);
		return solid;
	}
}
