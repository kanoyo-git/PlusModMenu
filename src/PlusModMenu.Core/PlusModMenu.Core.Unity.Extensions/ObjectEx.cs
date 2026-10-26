using System;
using System.Collections;
using UnityEngine;

namespace PlusModMenu.Core.Unity.Extensions;

public static class ObjectEx
{
	public static IEnumerator WaitForInitialization<TObject>(this TObject @this, Action<TObject> then) where TObject : UnityEngine.Object
	{
		while (true)
		{
			yield return null;
			try
			{
				_ = @this.name;
			}
			catch (UnityException)
			{
				continue;
			}
			catch (NullReferenceException) when (@this == null)
			{
				yield break;
			}
			catch (Exception msg)
			{
				CoreLogger.Log("A general error occured, while trying to wait for the initialization of a Unity object.", LogType.Error);
				CoreLogger.Log(msg, LogType.Error);
				yield break;
			}
			break;
		}
		then(@this);
	}
}
