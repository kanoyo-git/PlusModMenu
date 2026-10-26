using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace PlusModMenu.Api.Collectors;

public abstract class MonoBehaviourCollectorItem : MonoBehaviour
{
	internal void CreateInternals(Type collectorGeneric, out MethodInfo internalAdd, out MethodInfo internalRemove)
	{
		Type type = typeof(MonoBehaviourCollector<, >).MakeGenericType(collectorGeneric, GetType());
		internalAdd = type.GetMethod("InternalAdd", AccessTools.all);
		internalRemove = type.GetMethod("InternalRemove", AccessTools.all);
		if (internalAdd == null || internalRemove == null)
		{
			throw new MissingMethodException("Could not find required internal method.");
		}
	}

	internal abstract void Create(object item);
}
