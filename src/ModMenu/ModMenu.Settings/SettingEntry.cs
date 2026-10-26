using System;
using System.Collections.Generic;
using PlusModMenu.Api.Settings;

namespace ModMenu.Settings;

public sealed class SettingEntry : ISettingHandle
{
	private UIText _label;

	private bool _visible = true;

	private bool _defaultCaptured;

	private int _notifications;

	public SettingKind Kind { get; }

	public UIText Label
	{
		get
		{
			return _label;
		}
		set
		{
			_label = value;
			Group.Page.Registry.RaisePageChanged(Group.Page);
		}
	}

	public UIText Help { get; }

	public SettingsGroup Group { get; }

	public object Owner { get; }

	public string MemberName { get; }

	public Func<object> Getter { get; }

	public Action<object> Setter { get; }

	public Action Invoke { get; }

	public NumberOptions Number { get; }

	public Func<IReadOnlyList<UIText>> Options { get; }

	public OrderAttribute Order { get; }

	public bool HasDefault { get; }

	public object Default { get; private set; }

	public bool IsModified => HasDefault && _defaultCaptured && !Equals(GetValue(), Default);

	public bool Visible
	{
		get
		{
			return _visible;
		}
		set
		{
			if (_visible != value)
			{
				_visible = value;
				Group.Page.Registry.RaisePageChanged(Group.Page);
			}
		}
	}

	public bool Available { get; private set; } = true;

	public UIText UnavailableReason { get; private set; }

	public bool IsRemoved { get; internal set; }

	public SettingEntry Lock { get; internal set; }

	public event Action Changed;

	internal SettingEntry(SettingsGroup group, SettingKind kind, UIText label, UIText help, object owner, string memberName, Func<object> getter, Action<object> setter, Action invoke, NumberOptions number, bool hasDefault, Func<IReadOnlyList<UIText>> options, OrderAttribute order)
	{
		Group = group;
		Kind = kind;
		_label = label;
		Help = help;
		Owner = owner;
		MemberName = memberName;
		Getter = getter;
		Setter = setter;
		Invoke = invoke;
		Number = number;
		HasDefault = hasDefault;
		Options = options;
		Order = order;
	}

	public object GetValue()
	{
		return Getter?.Invoke();
	}

	public void SetValue(object value)
	{
		var before = _notifications;
		Setter?.Invoke(value);
		if (_notifications == before)
		{
			NotifyChanged();
		}
	}

	public void NotifyChanged()
	{
		_notifications++;
		var changed = Changed;
		if (changed == null)
		{
			return;
		}
		foreach (Action handler in changed.GetInvocationList())
		{
			try
			{
				handler();
			}
			catch (Exception e)
			{
				Group.Page.Registry.Log?.Invoke(e);
			}
		}
	}

	public void CaptureDefault()
	{
		if (HasDefault && Getter != null)
		{
			Default = GetValue();
			_defaultCaptured = true;
		}
	}

	public void ResetToDefault()
	{
		if (HasDefault && _defaultCaptured && Setter != null)
		{
			SetValue(Default);
		}
	}

	public void SetAvailable(bool available, UIText reason = default)
	{
		if (Available != available || UnavailableReason != reason)
		{
			Available = available;
			UnavailableReason = reason;
			Group.Page.Registry.RaisePageChanged(Group.Page);
		}
	}

	public void Refresh()
	{
		NotifyChanged();
	}

	public void Remove()
	{
		Group.RemoveEntry(this);
	}
}
