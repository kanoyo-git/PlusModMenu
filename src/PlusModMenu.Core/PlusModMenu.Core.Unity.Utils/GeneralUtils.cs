using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlusModMenu.Core.Unity.Utils;

public class GeneralUtils
{
	public static T[] FindObjectsOfTypeInScene<T>()
	{
		GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
		List<T> list = new List<T>();
		GameObject[] array = rootGameObjects;
		foreach (GameObject gameObject in array)
		{
			list.AddRange(gameObject.GetComponentsInChildren<T>(includeInactive: true));
		}
		return list.ToArray();
	}

	public static T? FindObjectOfTypeInScene<T>()
	{
		GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
		for (int i = 0; i < rootGameObjects.Length; i++)
		{
			T componentInChildren = rootGameObjects[i].GetComponentInChildren<T>(includeInactive: true);
			if (componentInChildren != null)
			{
				return componentInChildren;
			}
		}
		return default;
	}

	public static bool ObjectOfTypeExistsInScene<T>()
	{
		return (from gameObject in SceneManager.GetActiveScene().GetRootGameObjects()
			select gameObject.GetComponentInChildren<T>(includeInactive: true)).Any((T match) => match != null);
	}

	public static bool ObjectOfTypeExistsInScene(Type t)
	{
		return (from gameObject in SceneManager.GetActiveScene().GetRootGameObjects()
			select gameObject.GetComponentInChildren(t, includeInactive: true)).Any((Component match) => match != null);
	}
}
