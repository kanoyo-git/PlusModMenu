using System;

namespace ModMenu.Hl1;

public static class Hl1Movement
{
	public static Hl1Vec Tick(Hl1State s, Hl1Input input, IHl1World world, float dt)
	{
		Duck(s, input.Duck, world, dt);

		var yaw = input.Yaw * (float)Math.PI / 180;
		var forward = new Hl1Vec((float)Math.Sin(yaw), 0, (float)Math.Cos(yaw));
		var right = new Hl1Vec((float)Math.Cos(yaw), 0, -(float)Math.Sin(yaw));
		var forwardMove = Clamp(input.Forward) * Hl1Constants.InputSpeed;
		var sideMove = Clamp(input.Side) * Hl1Constants.InputSpeed;
		var asked = (float)Math.Sqrt(forwardMove * forwardMove + sideMove * sideMove);
		if (asked > Hl1Constants.MaxSpeed)
		{
			forwardMove *= Hl1Constants.MaxSpeed / asked;
			sideMove *= Hl1Constants.MaxSpeed / asked;
		}
		if (s.Ducked)
		{
			forwardMove *= Hl1Constants.DuckSpeedScale;
			sideMove *= Hl1Constants.DuckSpeedScale;
		}
		var wishVelocity = forward * forwardMove + right * sideMove;
		var wishSpeed = wishVelocity.Length;
		var wishDir = wishVelocity.Normalized;
		if (wishSpeed > Hl1Constants.MaxSpeed)
		{
			wishSpeed = Hl1Constants.MaxSpeed;
		}

		if (!s.OnGround)
		{
			AddVerticalVelocity(s, -Hl1Constants.Gravity * dt / 2);
		}
		if (input.Jump && s.OnGround)
		{
			s.OnGround = false;
			s.Velocity = s.Velocity.WithY(Hl1Constants.JumpSpeed);
		}
		if (s.OnGround)
		{
			s.Velocity = s.Velocity.WithY(0);
			Friction(s, world, dt);
			Accelerate(s, wishDir, wishSpeed, Hl1Constants.Accelerate, dt);
		}
		else
		{
			AirAccelerate(s, wishDir, wishSpeed, Hl1Constants.AirAccelerate, dt);
		}

		MoveVertically(s, world, dt);
		if (!s.OnGround)
		{
			AddVerticalVelocity(s, -Hl1Constants.Gravity * dt / 2);
		}
		return s.Velocity.Flat;
	}

	private static void MoveVertically(Hl1State s, IHl1World world, float dt)
	{
		var hull = s.HullHeight;
		var vz = s.Velocity.Y;
		var feet = s.Feet + vz * dt;
		if (vz > 0)
		{
			var ceiling = world.CeilingAbove(s.Feet, hull);
			if (feet + hull > ceiling)
			{
				feet = Math.Max(s.Feet, ceiling - hull);
				vz = 0;
			}
		}
		var ground = world.GroundUnder(s.Feet, hull) ?? float.NegativeInfinity;
		if (feet <= ground)
		{
			feet = ground;
			if (vz < 0) vz = 0;
		}
		s.OnGround = vz <= Hl1Constants.MaxGroundVz && feet - ground <= Hl1Constants.GroundProbe;
		if (s.OnGround)
		{
			feet = ground;
			vz = 0;
		}
		s.Feet = feet;
		s.Velocity = s.Velocity.WithY(vz);
	}

	private static void Duck(Hl1State s, bool duck, IHl1World world, float dt)
	{
		var pressed = duck && !s.DuckHeld;
		s.DuckHeld = duck;
		if (duck)
		{
			if (s.Unducking)
			{
				s.Unducking = false;
				if (pressed && !s.OnGround)
				{
					s.Feet = System.Math.Max(s.Feet, System.Math.Min(s.Feet + Hl1Constants.DuckOffset, Hl1Constants.Ceiling - Hl1Constants.DuckHeight));
					Unstick(s, world);
				}
				return;
			}
			if (pressed && !s.Ducked && !s.InDuck)
			{
				s.InDuck = true;
				s.DuckTimer = 0;
			}
			if (s.InDuck)
			{
				s.DuckTimer += dt;
				if (!s.OnGround || s.DuckTimer >= Hl1Constants.DuckTime - 1e-4f)
				{
					FinishDuck(s, world);
				}
			}
			return;
		}
		if (s.Unducking)
		{
			s.UnduckTimer -= dt;
			if (s.UnduckTimer > 1e-4f)
			{
				return;
			}
			if (s.OnGround ? world.HullClear(s.Feet, Hl1Constants.StandHeight) : TryUnduckInAir(s, world))
			{
				s.Unducking = false;
				s.Ducked = false;
			}
			else if (s.OnGround && !world.HullClear(s.Feet, Hl1Constants.DuckHeight))
			{
				Unstick(s, world);
			}
			return;
		}
		if (s.InDuck)
		{
			if (s.OnGround && world.HullClear(s.Feet + Hl1Constants.DuckOffset, Hl1Constants.StandHeight))
			{
				s.Feet += Hl1Constants.DuckOffset;
				s.OnGround = false;
			}
			s.InDuck = false;
			s.DuckTimer = 0;
			return;
		}
		if (!s.Ducked)
		{
			return;
		}
		if (s.OnGround || !TryUnduckInAir(s, world))
		{
			s.Unducking = true;
			s.UnduckTimer = s.OnGround ? Hl1Constants.UnduckTime : 0;
			return;
		}
		s.Ducked = false;
	}

	private static bool TryUnduckInAir(Hl1State s, IHl1World world)
	{
		var ground = world.GroundUnder(s.Feet, Hl1Constants.DuckHeight) ?? float.NegativeInfinity;
		var feet = System.Math.Max(s.Feet - Hl1Constants.DuckOffset, ground);
		if (!world.HullClear(feet, Hl1Constants.StandHeight))
		{
			return false;
		}
		s.Feet = feet;
		return true;
	}

	private static void FinishDuck(Hl1State s, IHl1World world)
	{
		s.InDuck = false;
		s.DuckTimer = 0;
		s.Ducked = true;
		if (!s.OnGround)
		{
			s.Feet += Hl1Constants.DuckOffset;
			Unstick(s, world);
		}
	}

	private static void Unstick(Hl1State s, IHl1World world)
	{
		var start = s.Feet;
		for (var i = 0; i < Hl1Constants.UnstickSteps && !world.HullClear(s.Feet, Hl1Constants.DuckHeight); i++)
		{
			if (s.Feet + Hl1Constants.UnstickStep + Hl1Constants.DuckHeight > Hl1Constants.Ceiling + 1e-4f)
			{
				s.Feet = start;
				return;
			}
			s.Feet += Hl1Constants.UnstickStep;
		}
	}

	private static void AddVerticalVelocity(Hl1State s, float dv) => s.Velocity = s.Velocity.WithY(s.Velocity.Y + dv);

	private static float Clamp(float x) => x < -1 ? -1 : x > 1 ? 1 : x;

	public static void Friction(Hl1State s, IHl1World world, float dt)
	{
		var speed = s.Velocity.Flat.Length;
		if (speed < 0.1f * Hl1Constants.K)
		{
			return;
		}
		var friction = Hl1Constants.Friction;
		if (world.EdgeAhead(s.Feet, s.Velocity.Flat.Normalized))
		{
			friction *= Hl1Constants.EdgeFriction;
		}
		var control = speed < Hl1Constants.StopSpeed ? Hl1Constants.StopSpeed : speed;
		var newSpeed = speed - control * friction * dt;
		if (newSpeed < 0)
		{
			newSpeed = 0;
		}
		s.Velocity = s.Velocity * (newSpeed / speed);
	}

	public static void Accelerate(Hl1State s, Hl1Vec wishDir, float wishSpeed, float accel, float dt)
	{
		var add = wishSpeed - Hl1Vec.Dot(s.Velocity, wishDir);
		if (add <= 0)
		{
			return;
		}
		var accelSpeed = accel * dt * wishSpeed;
		s.Velocity = s.Velocity + wishDir * Math.Min(accelSpeed, add);
	}

	public static void AirAccelerate(Hl1State s, Hl1Vec wishDir, float wishSpeed, float accel, float dt)
	{
		var capped = wishSpeed > Hl1Constants.AirWishCap ? Hl1Constants.AirWishCap : wishSpeed;
		var add = capped - Hl1Vec.Dot(s.Velocity, wishDir);
		if (add <= 0)
		{
			return;
		}
		var accelSpeed = accel * wishSpeed * dt;
		s.Velocity = s.Velocity + wishDir * Math.Min(accelSpeed, add);
	}

	public static void ClipToWall(Hl1State s, Hl1Vec normal)
	{
		var backoff = Hl1Vec.Dot(s.Velocity, normal);
		if (backoff >= 0)
		{
			return;
		}
		var v = s.Velocity - normal * backoff;
		s.Velocity = new Hl1Vec(Near0(v.X), Near0(v.Y), Near0(v.Z));
	}

	private static float Near0(float x) => x > -Hl1Constants.StopEpsilon && x < Hl1Constants.StopEpsilon ? 0 : x;
}
