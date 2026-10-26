using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Collectors;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Collectors;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables;

[DataContract]
internal class StudentShuffleEventModifiers : AbstractEventModifiers<StudentEvent>
{
	private int _totalFollowers = 128;

	private float _pushSpeed = 15f;

	[DataMember]
	[Label<Strings>("MIN_EVENT_TIME")]
	[Help<HelpText>("MIN_EVENT_TIME_TITLE", "MIN_EVENT_TIME_TEXT")]
	[Group<SideMenuGroupStudentShuffleEventModifiersGeneral>]
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
	[Group<SideMenuGroupStudentShuffleEventModifiersGeneral>]
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
	[Label<Strings>("STUDENT_TOTAL_FOLLOWERS")]
	[Help<HelpText>("STUDENT_TOTAL_FOLLOWERS_TITLE", "STUDENT_TOTAL_FOLLOWERS_TEXT")]
	[Group<SideMenuGroupStudentShuffleEventModifiersGeneral>]
	public virtual int TotalFollowers
	{
		get
		{
			return _totalFollowers;
		}
		set
		{
			_totalFollowers = value;
			ApplyToAll();
		}
	}

	[DataMember]
	[Label<Strings>("STUDENT_PUSH_SPEED")]
	[Help<HelpText>("STUDENT_PUSH_SPEED_TITLE", "STUDENT_PUSH_SPEED_TEXT")]
	[Group<SideMenuGroupStudentShuffleEventModifiersGeneral>]
	public virtual float PushSpeed
	{
		get
		{
			return _pushSpeed;
		}
		set
		{
			_pushSpeed = value;
			ApplyToAll();
		}
	}

	[Label<Strings>("EVENT_START")]
	[Help<HelpText>("EVENT_START_TITLE", "EVENT_START_TEXT")]
	[Group<SideMenuGroupStudentShuffleEventModifiersGeneral>]
	public override void StartEvent()
	{
		EnsureSpawnRoom();
		base.StartEvent();
	}

	[Label<Strings>("EVENT_STOP")]
	[Help<HelpText>("EVENT_STOP_TITLE", "EVENT_STOP_TEXT")]
	[Group<SideMenuGroupStudentShuffleEventModifiersGeneral>]
	public override void StopEvent()
	{
		base.StopEvent();
	}

	internal override void ApplyTo(RandomEvent randomEvent)
	{
		if (randomEvent is not StudentEvent)
		{
			return;
		}
		Set(randomEvent, "totalFollowersToSpawn", Mathf.Clamp(_totalFollowers, 0, 512));
		Set(randomEvent, "pushSpeed", _pushSpeed);
	}
}
