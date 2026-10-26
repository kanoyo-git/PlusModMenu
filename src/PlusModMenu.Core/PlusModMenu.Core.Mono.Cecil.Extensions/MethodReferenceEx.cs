using Mono.Cecil;

namespace PlusModMenu.Core.Mono.Cecil.Extensions;

public static class MethodReferenceEx
{
	public static MethodReference MakeGeneric(this MethodReference @this, params TypeReference[] arguments)
	{
		MethodReference methodReference = new MethodReference(@this.Name, @this.ReturnType)
		{
			DeclaringType = @this.DeclaringType.MakeGeneric(arguments),
			HasThis = @this.HasThis,
			ExplicitThis = @this.ExplicitThis,
			CallingConvention = @this.CallingConvention
		};
		foreach (ParameterDefinition parameter in @this.Parameters)
		{
			methodReference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));
		}
		foreach (GenericParameter genericParameter in @this.GenericParameters)
		{
			methodReference.GenericParameters.Add(new GenericParameter(genericParameter.Name, methodReference));
		}
		return methodReference;
	}
}
