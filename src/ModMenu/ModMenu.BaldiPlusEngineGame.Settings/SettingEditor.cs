using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModMenu.Settings;
using ModMenu.Settings.Editors;
using PlusModMenu.Api;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces.Services;
using PlusModMenu.Api.Settings;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Settings;

[EditorControl(typeof(bool), typeof(LabelAttribute), typeof(GroupAttribute))]
[EditorControl(typeof(int), typeof(LabelAttribute), typeof(GroupAttribute))]
[EditorControl(typeof(float), typeof(LabelAttribute), typeof(GroupAttribute))]
[EditorControl(typeof(double), typeof(LabelAttribute), typeof(GroupAttribute))]
[EditorControl(typeof(string), typeof(LabelAttribute), typeof(GroupAttribute))]
[EditorControl(typeof(Enum), typeof(LabelAttribute), typeof(GroupAttribute))]
[EditorControl(typeof(Color), typeof(LabelAttribute), typeof(GroupAttribute))]
[EditorControl(typeof(Color32), typeof(LabelAttribute), typeof(GroupAttribute))]
[EditorControl(typeof(Vector2), typeof(LabelAttribute), typeof(GroupAttribute))]
[EditorControl(typeof(void), typeof(LabelAttribute), typeof(GroupAttribute))]
public class SettingEditor : IEditor
{
	private static readonly MethodInfo BoxedAccessorsMethod = typeof(SettingEditor).GetMethod(nameof(BoxedAccessors), BindingFlags.Static | BindingFlags.NonPublic);

	public SettingEntry Entry { get; protected set; }

	public virtual void Initialize(IMetadata metadata)
	{
		var group = metadata.Member.GetCustomAttribute<GroupAttribute>(true)?.Group;
		if (group == null)
		{
			return;
		}
		Entry = CreateEntry(SettingsRegistryService.Registry, SettingsRegistryService.Registry.GetGroup(group) as SettingsGroup, metadata, this is RangeEditor);
	}

	public void SetValueWithoutNotify(object input)
	{
		Entry?.NotifyChanged();
	}

	internal static SettingEntry CreateEntry(SettingsRegistry registry, SettingsGroup group, IMetadata metadata, bool forceRange, UIText? labelOverride = null)
	{
		var member = metadata.Member;
		Func<object> get = null;
		Action<object> set = null;
		Action invoke = null;
		Type valueType;
		switch (metadata)
		{
		case MethodMetadata method:
			valueType = typeof(void);
			invoke = method.Method;
			break;
		case PropertyMetadata property:
			valueType = property.GetType().IsGenericType ? property.GetType().GetGenericArguments()[0] : ((PropertyInfo)member).PropertyType;
			var accessors = (Tuple<Func<object>, Action<object>>)BoxedAccessorsMethod.MakeGenericMethod(valueType).Invoke(null, new object[] { property });
			get = accessors.Item1;
			set = accessors.Item2;
			break;
		default:
			return null;
		}
		var range = member.GetCustomAttribute<ValueRangeAttribute>(true);
		var kind = SettingKinds.For(valueType, range?.Min, range?.Max, forceRange);
		if (kind == null)
		{
			Debug.LogWarning("Plus Mod Menu: " + member.DeclaringType?.Name + "." + member.Name + " has an unsupported type " + valueType + ".");
			return null;
		}
		Func<IReadOnlyList<UIText>> options = null;
		if (kind == SettingKind.Choice)
		{
			var values = Enum.GetValues(valueType).Cast<object>().ToArray();
			IReadOnlyList<UIText> names = values.Select(v => UIText.Literal(v.ToString())).ToList();
			options = () => names;
			var getEnum = get;
			var setEnum = set;
			get = () => Array.IndexOf(values, getEnum());
			set = v => setEnum(values[Mathf.Clamp(Convert.ToInt32(v), 0, values.Length - 1)]);
		}
		var number = new NumberOptions
		{
			Integer = valueType == typeof(int),
			Min = range?.Min,
			Max = range?.Max,
			Step = member.GetCustomAttribute<StepAttribute>(true)?.Step
		};
		var hasDefault = kind != SettingKind.Action && !member.IsDefined(typeof(NoDefaultAttribute), true);
		var label = labelOverride ?? member.GetCustomAttribute<LabelAttribute>(true)?.ToUIText() ?? UIText.Literal(member.Name);
		var help = member.GetCustomAttribute<HelpAttribute>(true)?.TextUI ?? default;
		var entry = registry.AddMember(group, kind.Value, metadata.Instance, member, label, help, get, set, invoke, number, hasDefault, options, member.GetCustomAttribute<OrderAttribute>(true));
		var condition = member.GetCustomAttribute<ConditionEventAttribute>(true);
		if (condition != null)
		{
			var reason = UIText.Resource<Strings>("NOT_AVAILABLE_NOW");
			entry.SetAvailable(condition.InitiallyActive, reason);
			Service.Get<IEventService>().Subscribe(condition.EventType, (object x) =>
			{
				entry.SetAvailable(condition.ConditionMethod?.Invoke(x) ?? false, reason);
			});
		}
		return entry;
	}

	private static Tuple<Func<object>, Action<object>> BoxedAccessors<T>(PropertyMetadata property)
	{
		var typed = (PropertyMetadata<T>)property;
		return Tuple.Create<Func<object>, Action<object>>(() => typed.Get(), v => typed.Set((T)v));
	}
}

[EditorControl(null, typeof(LabelAttribute), typeof(GroupAttribute))]
public class RangeEditor : SettingEditor
{
}

[EditorControl(null)]
public class LockEditor : SettingEditor
{
	public override void Initialize(IMetadata metadata)
	{
		if (!(metadata is PropertyMetadata<bool> property))
		{
			return;
		}
		var name = metadata.Member.Name;
		var baseName = name.StartsWith("Lock", StringComparison.Ordinal) ? name.Substring(4) : name;
		Entry = SettingsRegistryService.Registry.AttachLock(metadata.Instance, baseName, property.Get, property.Set);
	}
}
