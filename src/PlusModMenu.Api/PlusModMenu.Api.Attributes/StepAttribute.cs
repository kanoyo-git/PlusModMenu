using System;

namespace PlusModMenu.Api.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class StepAttribute : Attribute
{
	public double Step { get; }

	public StepAttribute(double step)
	{
		Step = step;
	}
}
