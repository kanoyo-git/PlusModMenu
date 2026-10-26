using System.Collections.Generic;
using PlusModMenu.Core.Unity;
using ModMenu.BaldiPlusEngineGame.Patches;
using ModMenu.Hl1;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Hl1;

internal sealed class Hl1Controller : MonoBehaviour
{
	private readonly Hl1Stepper _stepper = new();

	private readonly UnityWorld _world = new();

	private readonly List<Vector3> _wallNormals = new();

	private Vector2 _absolute;

	private Vector2 _delta;

	private PlayerBody _body = null!;

	private Hl1Vec _horizontal;

	private bool _jumpLatch;

	public Hl1State State { get; internal set; } = new();

	public float SpeedHl => State.SpeedHl;

	internal Hl1Input? TestInput { get; set; }

	internal UnityWorld World => _world;

	internal Hl1Input LastInput { get; private set; }

	public bool GameControls => _body.GameControls;

	internal string GameControlsReason => _body.GameControlsReason;

	public static Hl1Controller Attach(PlayerManager player)
	{
		var controller = player.GetComponent<Hl1Controller>() ?? player.gameObject.AddComponent<Hl1Controller>();
		controller._body = new PlayerBody(player);
		return controller;
	}

	public void Detach()
	{
		_body.Restore();
		Hl1CollisionPatch.RestoreAll();
		DestroyImmediate(this);
	}

	public void Step(PlayerMovement movement)
	{
		var player = _body.Player;
		_world.Position = player.transform.position;
		_world.HalfWidth = movement.cc.radius;
		if (Time.timeScale == 0)
		{
			movement.Entity.UpdateInternalMovement(Vector3.zero);
			return;
		}
		if (_body.GameControls)
		{
			Suspend();
			movement.Entity.UpdateInternalMovement(Vector3.zero);
			return;
		}
		var input = TestInput ?? ReadInput(movement);
		LastInput = input;
		foreach (var normal in _wallNormals)
		{
			Hl1Movement.ClipToWall(State, new Hl1Vec(normal.x, 0, normal.z));
		}
		_wallNormals.Clear();
		var ticks = _stepper.Advance(Time.deltaTime * player.PlayerTimeScale);
		_jumpLatch |= input.Jump;
		for (var i = 0; i < ticks; i++)
		{
			var tickInput = input;
			tickInput.Jump |= _jumpLatch;
			_jumpLatch = false;
			_horizontal = Hl1Movement.Tick(State, tickInput, _world, Hl1Constants.TickSeconds);
		}
		if (ticks > 0)
		{
			_horizontal = State.Velocity.Flat;
		}
		if (State.OnGround)
		{
			State.Feet = _world.StepUp(State.Feet, State.HullHeight);
		}
		Hl1CollisionPatch.Feet = State.Feet;
		Hl1CollisionPatch.Grounded = State.OnGround;
		_body.Apply(State.Feet, State.EyeHeight);
		movement.Entity.UpdateInternalMovement(new Vector3(_horizontal.X, 0, _horizontal.Z) * player.PlayerTimeScale);
	}

	public void Suspend()
	{
		State = new Hl1State();
		_stepper.Reset();
		_jumpLatch = false;
		_wallNormals.Clear();
		Hl1CollisionPatch.Feet = 0;
		_body.Apply(0, Hl1Constants.StandEye);
	}

	private Hl1Input ReadInput(PlayerMovement movement)
	{
		var input = new Hl1Input { Yaw = movement.transform.eulerAngles.y };
		if (!movement.Entity.InteractionDisabled)
		{
			Singleton<InputManager>.Instance.GetAnalogInput(movement.movementAnalogData, out _absolute, out _delta);
			input.Forward = _absolute.y;
			input.Side = _absolute.x * (movement.pm.reversed ? -1 : 1);
			input.Jump = Input.GetKey(Config.Instance.JumpKey);
			input.Duck = Input.GetKey(Config.Instance.Hl1DuckKey);
		}
		return input;
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		var normal = hit.normal;
		normal.y = 0;
		if (normal.sqrMagnitude > 0.25f)
		{
			_wallNormals.Add(normal.normalized);
		}
	}

	private void OnDestroy()
	{
		if (_body != null && Hl1Mode.Controller == this)
		{
			_body.Restore();
			Hl1CollisionPatch.RestoreAll();
		}
	}
}
