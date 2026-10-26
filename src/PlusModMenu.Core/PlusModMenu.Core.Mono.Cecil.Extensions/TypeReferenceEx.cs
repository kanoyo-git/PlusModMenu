using System;
using Mono.Cecil;

namespace PlusModMenu.Core.Mono.Cecil.Extensions;

public static class TypeReferenceEx
{
	public static TypeReference MakeGeneric(this TypeReference @this, params TypeReference[] arguments)
	{
		if (@this.GenericParameters.Count != arguments.Length)
		{
			throw new ArgumentException("The provided generic types do not match up with the required amount.");
		}
		GenericInstanceType genericInstanceType = new GenericInstanceType(@this);
		foreach (TypeReference item in arguments)
		{
			genericInstanceType.GenericArguments.Add(item);
		}
		return genericInstanceType;
	}
}
