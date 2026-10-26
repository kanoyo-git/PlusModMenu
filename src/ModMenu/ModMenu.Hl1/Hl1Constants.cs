namespace ModMenu.Hl1;

public static class Hl1Constants
{
	public const float K = 0.078f;

	public const float MaxSpeed = 320 * K;

	public const float InputSpeed = 400 * K;

	public const float AirWishCap = 30 * K;

	public const float Accelerate = 10;

	public const float AirAccelerate = 10;

	public const float Friction = 4;

	public const float EdgeFriction = 2;

	public const float StopSpeed = 100 * K;

	public const float Gravity = 800 * K;

	public const float JumpSpeed = 268.3282f * K;

	public const float StepSize = 18 * K;

	public const float StandHeight = 72 * K;

	public const float DuckHeight = 36 * K;

	public const float HullWidth = 32 * K;

	public const float StandEye = 64 * K;

	public const float DuckEye = 30 * K;

	public const float DuckOffset = 18 * K;

	public const float DuckTime = 0.4f;

	public const float UnduckTime = 0.2f;

	public const float DuckSpeedScale = 0.333f;

	public const float GroundProbe = 2 * K;

	public const float MaxGroundVz = 180 * K;

	public const float EdgeProbeAhead = 16 * K;

	public const float EdgeProbeDown = 34 * K;

	public const float UnstickStep = 1 * K;

	public const int UnstickSteps = 72;

	public const float StopEpsilon = 0.1f * K;

	public const float Ceiling = 10f;

	public const float TickSeconds = 0.01f;

	public const int MaxTicksPerFrame = 10;

	public static float ToHl(float baldi) => baldi / K;
}
