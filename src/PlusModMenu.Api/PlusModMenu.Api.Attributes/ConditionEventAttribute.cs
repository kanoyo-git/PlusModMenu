using System;
using System.Collections.Generic;
using System.Reflection;
using PlusModMenu.Core.Common.Utils;

namespace PlusModMenu.Api.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public class ConditionEventAttribute<TEvent> : ConditionEventAttribute
{
	public ConditionEventAttribute(bool initiallyActive, Type conditionCheckerType, string conditionCheckerMethod)
		: base(initiallyActive, typeof(TEvent), conditionCheckerType, conditionCheckerMethod)
	{
	}
}
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public class ConditionEventAttribute : Attribute
{
	private static readonly Dictionary<(Type, string), Func<object?, bool>> _conditionMethods = new Dictionary<(Type, string), Func<object, bool>>();

	public bool InitiallyActive { get; }

	public Type EventType { get; }

	public Func<object?, bool>? ConditionMethod { get; }

	public ConditionEventAttribute(bool initiallyActive, Type eventType, Type conditionCheckerType, string conditionCheckerMethod)
	{
		InitiallyActive = initiallyActive;
		EventType = eventType;
		if (!_conditionMethods.TryGetValue((conditionCheckerType, conditionCheckerMethod), out Func<object, bool> value))
		{
			value = ExpressionUtils.CompileStaticMethod<Func<object, bool>>(conditionCheckerType.GetMethod(conditionCheckerMethod, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[1] { typeof(object) }, null), new Type[1] { typeof(object) });
			_conditionMethods.Add((conditionCheckerType, conditionCheckerMethod), value);
		}
		ConditionMethod = value;
	}
}
