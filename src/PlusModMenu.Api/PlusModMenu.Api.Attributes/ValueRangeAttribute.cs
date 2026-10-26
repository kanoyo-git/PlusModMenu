using System;

namespace PlusModMenu.Api.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public class ValueRangeAttribute : Attribute
{
	public double Min { get; }

	public double Max { get; }

	public ValueRangeAttribute(int min, int max)
	{
		Min = min;
		Max = max;
	}

	public ValueRangeAttribute(float min, float max)
	{
		Min = min;
		Max = max;
	}

	public ValueRangeAttribute(double min, double max)
	{
		Min = min;
		Max = max;
	}
}
