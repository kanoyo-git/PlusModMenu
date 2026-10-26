namespace ModMenu.Hl1;

public sealed class Hl1State
{
	public Hl1Vec Velocity;

	public float Feet;

	public bool OnGround = true;

	public bool Ducked;

	public bool InDuck;

	public float DuckTimer;

	public bool Unducking;

	public float UnduckTimer;

	public bool DuckHeld;

	public float HullHeight => Ducked ? Hl1Constants.DuckHeight : Hl1Constants.StandHeight;

	public float EyeHeight
	{
		get
		{
			if (Ducked) return Hl1Constants.DuckEye;
			if (!InDuck) return Hl1Constants.StandEye;
			var t = DuckTimer / Hl1Constants.DuckTime;
			return Hl1Constants.StandEye + (Hl1Constants.DuckEye - Hl1Constants.StandEye) * (t > 1 ? 1 : t);
		}
	}

	public float SpeedHl => Hl1Constants.ToHl(Velocity.Flat.Length);
}
