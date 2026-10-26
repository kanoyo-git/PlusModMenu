using System;

namespace ModMenu.Hl1;

public readonly struct Hl1Vec
{
	public readonly float X;

	public readonly float Y;

	public readonly float Z;

	public Hl1Vec(float x, float y, float z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	public static readonly Hl1Vec Zero = new(0, 0, 0);

	public float Length => (float)Math.Sqrt(X * X + Y * Y + Z * Z);

	public Hl1Vec Flat => new(X, 0, Z);

	public Hl1Vec Normalized
	{
		get
		{
			var length = Length;
			return length > 1e-6f ? this * (1 / length) : Zero;
		}
	}

	public Hl1Vec WithY(float y) => new(X, y, Z);

	public static float Dot(Hl1Vec a, Hl1Vec b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

	public static Hl1Vec operator +(Hl1Vec a, Hl1Vec b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

	public static Hl1Vec operator -(Hl1Vec a, Hl1Vec b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

	public static Hl1Vec operator *(Hl1Vec a, float f) => new(a.X * f, a.Y * f, a.Z * f);

	public override string ToString() => $"({X:0.###}, {Y:0.###}, {Z:0.###})";
}
