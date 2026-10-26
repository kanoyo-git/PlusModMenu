using System;
using System.Diagnostics;

namespace PlusModMenu.Core.Common.Diag;

public static class Benchmark
{
	public static long Run(Action action, int iterations)
	{
		GC.Collect();
		action();
		Stopwatch stopwatch = Stopwatch.StartNew();
		for (int i = 0; i < iterations; i++)
		{
			action();
		}
		stopwatch.Stop();
		return stopwatch.ElapsedMilliseconds;
	}
}
