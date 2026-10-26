using System;

namespace ModMenu.Hl1;

public static class BoxPush
{
	public static (float x, float z)? Out(float x, float z, float radius, float minX, float maxX, float minZ, float maxZ)
	{
		var inside = x > minX && x < maxX && z > minZ && z < maxZ;
		if (inside)
		{
			var left = x - minX;
			var right = maxX - x;
			var back = z - minZ;
			var front = maxZ - z;
			var least = Math.Min(Math.Min(left, right), Math.Min(back, front));
			if (least == right) return (maxX + radius, z);
			if (least == left) return (minX - radius, z);
			if (least == front) return (x, maxZ + radius);
			return (x, minZ - radius);
		}
		var cx = Math.Max(minX, Math.Min(maxX, x));
		var cz = Math.Max(minZ, Math.Min(maxZ, z));
		var dx = x - cx;
		var dz = z - cz;
		var distance = (float)Math.Sqrt(dx * dx + dz * dz);
		if (distance >= radius) return null;
		return (cx + dx / distance * radius, cz + dz / distance * radius);
	}

	public static (float x, float z)? Step(float sx, float sz, float ex, float ez, float radius, float minX, float maxX, float minZ, float maxZ)
	{
		var x0 = minX - radius;
		var x1 = maxX + radius;
		var z0 = minZ - radius;
		var z1 = maxZ + radius;
		if (sx > x0 && sx < x1 && sz > z0 && sz < z1)
		{
			return Out(ex, ez, radius, minX, maxX, minZ, maxZ);
		}
		if (!Slab(sx, ex - sx, x0, x1, out var enterX, out var exitX) || !Slab(sz, ez - sz, z0, z1, out var enterZ, out var exitZ))
		{
			return null;
		}
		var enter = Math.Max(enterX, enterZ);
		var exit = Math.Min(exitX, exitZ);
		if (enter > exit || enter < 0f || enter >= 1f)
		{
			return null;
		}
		if (enterX >= enterZ)
		{
			return (ex > sx ? x0 : x1, ez);
		}
		return (ex, ez > sz ? z0 : z1);
	}

	private static bool Slab(float start, float delta, float low, float high, out float enter, out float exit)
	{
		if (delta == 0f)
		{
			enter = float.NegativeInfinity;
			exit = float.PositiveInfinity;
			return start > low && start < high;
		}
		var a = (low - start) / delta;
		var b = (high - start) / delta;
		enter = Math.Min(a, b);
		exit = Math.Max(a, b);
		return true;
	}
}
