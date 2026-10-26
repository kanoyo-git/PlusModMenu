using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using PlusModMenu.Api.Settings;

namespace ModMenu.Settings;

public sealed class SettingsRegistry : ISettingsRegistry
{
	private readonly List<SettingsSection> _sections = new List<SettingsSection>();

	private readonly Dictionary<Type, SettingsSection> _sectionsByType = new Dictionary<Type, SettingsSection>();

	private readonly Dictionary<Type, SettingsPage> _pagesByType = new Dictionary<Type, SettingsPage>();

	private readonly Dictionary<Type, SettingsGroup> _groupsByType = new Dictionary<Type, SettingsGroup>();

	private readonly ConditionalWeakTable<object, Dictionary<string, List<SettingEntry>>> _entriesByMember = new ConditionalWeakTable<object, Dictionary<string, List<SettingEntry>>>();

	private readonly ConditionalWeakTable<object, Dictionary<string, SettingEntry>> _pendingLocks = new ConditionalWeakTable<object, Dictionary<string, SettingEntry>>();

	private readonly List<SettingEntry> _locks = new List<SettingEntry>();

	private SettingsGroup _lockGroup;

	private SettingsGroup LockGroup => _lockGroup ??= new SettingsGroup(new SettingsPage(new SettingsSection(this, typeof(SettingsRegistry), default, null), null, default, null), null, default, null);

	public IDialogs Dialogs { get; }

	public IReadOnlyList<SettingsSection> Sections => OrderRules.Sort(_sections, s => s.Order);

	public Action<Exception> Log { get; set; }

	public event Action<SettingsPage> PageChanged;

	public IEnumerable<SettingEntry> AllEntries => _sections.SelectMany(s => s.PageList).SelectMany(p => p.Groups).SelectMany(g => g.AllEntries);

	public SettingsRegistry(IDialogs dialogs)
	{
		Dialogs = dialogs;
	}

	public void Build(IEnumerable<(Type type, UIText label, Type parentMenu, OrderAttribute order)> groups, IEnumerable<(Type type, UIText label, Type parentGroup, OrderAttribute order)> menus)
	{
		var groupList = groups.ToList();
		foreach (var (type, label, _, order) in groupList.Where(g => g.parentMenu == null))
		{
			var section = new SettingsSection(this, type, label, order);
			_sections.Add(section);
			_sectionsByType.Add(type, section);
		}
		foreach (var (type, label, parentGroup, order) in menus)
		{
			if (parentGroup == null || !_sectionsByType.TryGetValue(parentGroup, out var section))
			{
				throw new InvalidOperationException("Menu " + type.FullName + ": its parent group " + parentGroup?.FullName + " is not a top-level group.");
			}
			var page = new SettingsPage(section, type, label, order);
			section.PageList.Add(page);
			_pagesByType.Add(type, page);
		}
		foreach (var (type, label, parentMenu, order) in groupList.Where(g => g.parentMenu != null))
		{
			if (!_pagesByType.TryGetValue(parentMenu, out var page))
			{
				throw new InvalidOperationException("Group " + type.FullName + ": its parent menu " + parentMenu.FullName + " is not registered.");
			}
			_groupsByType.Add(type, page.AddGroup(type, label, order));
		}
	}

	public ISettingsPage GetPage(Type staticMenuType)
	{
		return _pagesByType.TryGetValue(staticMenuType, out var page) ? page : throw new InvalidOperationException("Menu " + staticMenuType?.FullName + " is not registered.");
	}

	public ISettingsGroup GetGroup(Type staticGroupType)
	{
		return FindGroup(staticGroupType);
	}

	private SettingsGroup FindGroup(Type groupType)
	{
		if (groupType != null && _groupsByType.TryGetValue(groupType, out var group))
		{
			return group;
		}
		if (groupType != null && _sectionsByType.TryGetValue(groupType, out var section))
		{
			var page = new SettingsPage(section, null, section.Label, new OrderAttribute(int.MinValue));
			section.PageList.Add(page);
			group = page.AddGroup(groupType, default, null);
			_groupsByType.Add(groupType, group);
			return group;
		}
		throw new InvalidOperationException("Group " + groupType?.FullName + " is not registered.");
	}

	public SettingEntry AddMember(Type groupType, SettingKind kind, object owner, MemberInfo member, UIText label, UIText help, Func<object> get, Action<object> set, Action invoke, NumberOptions number, bool hasDefault, Func<IReadOnlyList<UIText>> options, OrderAttribute order)
	{
		return AddMember(FindGroup(groupType), kind, owner, member, label, help, get, set, invoke, number, hasDefault, options, order);
	}

	public SettingEntry AddMember(SettingsGroup group, SettingKind kind, object owner, MemberInfo member, UIText label, UIText help, Func<object> get, Action<object> set, Action invoke, NumberOptions number, bool hasDefault, Func<IReadOnlyList<UIText>> options, OrderAttribute order)
	{
		return group.Add(new SettingEntry(group, kind, label, help, owner, member?.Name, get, set, invoke, number, hasDefault, options, order));
	}

	public SettingEntry AttachLock(object owner, string baseMemberName, Func<bool> get, Action<bool> set)
	{
		var lockEntry = new SettingEntry(LockGroup, SettingKind.Toggle, default, default, owner, "Lock" + baseMemberName, () => get(), v => set((bool)v), null, default, true, null, null);
		_locks.Add(lockEntry);
		var existing = Find(owner, baseMemberName);
		if (existing != null)
		{
			existing.Lock = lockEntry;
		}
		else
		{
			_pendingLocks.GetOrCreateValue(owner)[baseMemberName] = lockEntry;
		}
		return lockEntry;
	}

	internal void OnEntryAdded(SettingEntry entry)
	{
		if (entry.Owner == null || entry.MemberName == null)
		{
			return;
		}
		var members = _entriesByMember.GetOrCreateValue(entry.Owner);
		if (!members.TryGetValue(entry.MemberName, out var rows))
		{
			rows = members[entry.MemberName] = new List<SettingEntry>();
		}
		rows.Add(entry);
		if (_pendingLocks.TryGetValue(entry.Owner, out var pending) && pending.TryGetValue(entry.MemberName, out var lockEntry))
		{
			entry.Lock = lockEntry;
			pending.Remove(entry.MemberName);
		}
	}

	internal void OnEntryRemoved(SettingEntry entry)
	{
		if (entry.Owner != null && entry.MemberName != null && _entriesByMember.TryGetValue(entry.Owner, out var members)
			&& members.TryGetValue(entry.MemberName, out var rows))
		{
			rows.Remove(entry);
		}
	}

	internal void RaisePageChanged(SettingsPage page)
	{
		if (page.Section.Registry != this || PageChanged == null)
		{
			return;
		}
		foreach (Action<SettingsPage> handler in PageChanged.GetInvocationList())
		{
			try
			{
				handler(page);
			}
			catch (Exception e)
			{
				Log?.Invoke(e);
			}
		}
	}

	public SettingEntry Find(object owner, string memberName)
	{
		return FindAll(owner, memberName).FirstOrDefault();
	}

	private IEnumerable<SettingEntry> FindAll(object owner, string memberName)
	{
		return owner != null && _entriesByMember.TryGetValue(owner, out var members) && members.TryGetValue(memberName, out var rows) ? rows : Enumerable.Empty<SettingEntry>();
	}

	public void Refresh(object modifiers, string memberName)
	{
		foreach (var entry in FindAll(modifiers, memberName).ToList())
		{
			entry.NotifyChanged();
		}
	}

	public void RefreshAll()
	{
		foreach (var entry in AllEntries.Concat(_locks).ToList())
		{
			entry.NotifyChanged();
		}
	}

	public void CaptureDefaults()
	{
		foreach (var entry in AllEntries.Concat(_locks))
		{
			entry.CaptureDefault();
		}
	}
}
