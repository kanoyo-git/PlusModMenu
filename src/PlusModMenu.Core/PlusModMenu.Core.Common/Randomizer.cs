using System;

namespace PlusModMenu.Core.Common;

public class Randomizer
{
	private static readonly Random _random = new Random();

	public static void Array<T>(T[] items)
	{
		int num = items.Length;
		while (num > 1)
		{
			int num2 = _random.Next(num--);
			int num3 = num;
			int num4 = num2;
			T val = items[num2];
			T val2 = items[num];
			items[num3] = val;
			items[num4] = val2;
		}
	}
}
