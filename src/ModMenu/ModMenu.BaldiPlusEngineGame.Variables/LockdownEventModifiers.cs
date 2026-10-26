using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Collectors;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Collectors;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables;

[DataContract]
internal class LockdownEventModifiers : AbstractEventModifiers<LockdownEvent>
{
	private int _minDoors = 10;

	private int _maxDoors = 15;

	[DataMember]
	[Label<Strings>("MIN_EVENT_TIME")]
	[Help<HelpText>("MIN_EVENT_TIME_TITLE", "MIN_EVENT_TIME_TEXT")]
	[Group<SideMenuGroupLockdownEventModifiersGeneral>]
	public override float MinEventTime
	{
		get
		{
			return base.MinEventTime;
		}
		set
		{
			base.MinEventTime = value;
		}
	}

	[DataMember]
	[Label<Strings>("MAX_EVENT_TIME")]
	[Help<HelpText>("MAX_EVENT_TIME_TITLE", "MAX_EVENT_TIME_TEXT")]
	[Group<SideMenuGroupLockdownEventModifiersGeneral>]
	public override float MaxEventTime
	{
		get
		{
			return base.MaxEventTime;
		}
		set
		{
			base.MaxEventTime = value;
		}
	}

	[DataMember]
	[Label<Strings>("LOCKDOWN_MIN_DOORS")]
	[Help<HelpText>("LOCKDOWN_MIN_DOORS_TITLE", "LOCKDOWN_MIN_DOORS_TEXT")]
	[Group<SideMenuGroupLockdownEventModifiersGeneral>]
	public virtual int MinDoors
	{
		get
		{
			return _minDoors;
		}
		set
		{
			_minDoors = value;
			ApplyToAll();
		}
	}

	[DataMember]
	[Label<Strings>("LOCKDOWN_MAX_DOORS")]
	[Help<HelpText>("LOCKDOWN_MAX_DOORS_TITLE", "LOCKDOWN_MAX_DOORS_TEXT")]
	[Group<SideMenuGroupLockdownEventModifiersGeneral>]
	public virtual int MaxDoors
	{
		get
		{
			return _maxDoors;
		}
		set
		{
			_maxDoors = value;
			ApplyToAll();
		}
	}

	[Label<Strings>("EVENT_START")]
	[Help<HelpText>("EVENT_START_TITLE", "EVENT_START_TEXT")]
	[Group<SideMenuGroupLockdownEventModifiersGeneral>]
	public override void StartEvent()
	{
		base.StartEvent();
	}

	[Label<Strings>("EVENT_STOP")]
	[Help<HelpText>("EVENT_STOP_TITLE", "EVENT_STOP_TEXT")]
	[Group<SideMenuGroupLockdownEventModifiersGeneral>]
	public override void StopEvent()
	{
		base.StopEvent();
	}

	internal override void ApplyTo(RandomEvent randomEvent)
	{
		if (randomEvent is not LockdownEvent)
		{
			return;
		}
		int min = Mathf.Clamp(_minDoors, 0, 50);
		Set(randomEvent, "minDoors", min);
		Set(randomEvent, "maxDoors", Mathf.Clamp(_maxDoors, min + 1, 51));
	}
}
