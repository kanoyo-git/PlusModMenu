using System;

namespace PlusModMenu.Core.Common;

public class Helper
{
	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Math.Max(Math.Min(angle, max), min);
	}
}
