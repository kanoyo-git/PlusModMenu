using System;
using System.Collections;
using UnityEngine;

namespace PlusModMenu.Core.Unity;

public static class OtherUtils
{
	public static void Invoke(this MonoBehaviour self, Action action, float time)
	{
		self.StartCoroutine(ExecuteAfterTime(action, time));
	}

	private static IEnumerator ExecuteAfterTime(Action action, float time)
	{
		yield return new WaitForSeconds(time);
		action();
	}

	public static bool IsValid(this Vector3 @this)
	{
		if (!float.IsNaN(@this.x) && !float.IsNaN(@this.y))
		{
			return !float.IsNaN(@this.z);
		}
		return false;
	}

	public static IEnumerator WaitForDistance(Transform object1, Transform object2, float distance)
	{
		distance *= 5f;
		while ((bool)object1 && (bool)object2 && Vector3.Distance(object1.position, object2.position) < distance)
		{
			yield return null;
		}
	}
}
