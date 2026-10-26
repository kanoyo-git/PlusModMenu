using System;
using System.Collections.Generic;

namespace ModMenu.Settings;

public static class SpriteColors
{
	private const int Buckets = 18;

	private const float MinSaturation = 0.45f;

	private const float MinValue = 0.2f;

	public static (float r, float g, float b) Dominant(IReadOnlyList<(float r, float g, float b, float a)> pixels, (float r, float g, float b)? avoid = null)
	{
		var taken = -1;
		if (avoid is var (ar, ag, ab))
		{
			var (h, s, _) = ToHsv(ar, ag, ab);
			taken = s < MinSaturation ? -1 : Math.Min(Buckets - 1, (int)(h * Buckets));
		}
		var counts = new int[Buckets];
		var sums = new (float r, float g, float b)[Buckets];
		var opaque = 0;
		foreach (var (r, g, b, a) in pixels)
		{
			if (a < 0.5f)
			{
				continue;
			}
			opaque++;
			var (h, s, v) = ToHsv(r, g, b);
			if (s < MinSaturation || v < MinValue)
			{
				continue;
			}
			var bucket = Math.Min(Buckets - 1, (int)(h * Buckets));
			counts[bucket]++;
			sums[bucket] = (sums[bucket].r + r, sums[bucket].g + g, sums[bucket].b + b);
		}
		var best = -1;
		for (var i = 0; i < Buckets; i++)
		{
			if (taken >= 0 && Math.Min(Math.Abs(i - taken), Buckets - Math.Abs(i - taken)) <= 1)
			{
				continue;
			}
			if (best < 0 || counts[i] > counts[best])
			{
				best = i;
			}
		}
		if (best < 0 || counts[best] == 0 || counts[best] < opaque * 0.04f)
		{
			return (1f, 1f, 1f);
		}
		var n = counts[best];
		var (hue, sat, _) = ToHsv(sums[best].r / n, sums[best].g / n, sums[best].b / n);
		return FromHsv(hue, Math.Max(sat, 0.7f), 0.95f);
	}

	private static (float h, float s, float v) ToHsv(float r, float g, float b)
	{
		var max = Math.Max(r, Math.Max(g, b));
		var min = Math.Min(r, Math.Min(g, b));
		var d = max - min;
		var s = max <= 0f ? 0f : d / max;
		float h;
		if (d <= 1e-5f)
		{
			h = 0f;
		}
		else if (max == r)
		{
			h = (g - b) / d / 6f;
		}
		else if (max == g)
		{
			h = ((b - r) / d + 2f) / 6f;
		}
		else
		{
			h = ((r - g) / d + 4f) / 6f;
		}
		if (h < 0f)
		{
			h += 1f;
		}
		return (h, s, max);
	}

	private static (float r, float g, float b) FromHsv(float h, float s, float v)
	{
		var i = (int)Math.Floor(h * 6f);
		var f = h * 6f - i;
		var p = v * (1f - s);
		var q = v * (1f - f * s);
		var t = v * (1f - (1f - f) * s);
		return (i % 6) switch
		{
			0 => (v, t, p),
			1 => (q, v, p),
			2 => (p, v, t),
			3 => (p, q, v),
			4 => (t, p, v),
			_ => (v, p, q),
		};
	}
}
