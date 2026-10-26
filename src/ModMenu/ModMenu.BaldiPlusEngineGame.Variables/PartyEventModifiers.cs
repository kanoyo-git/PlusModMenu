using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Collectors;
using ModMenu.BaldiPlusEngineGame.Patches;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Collectors;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables;

[DataContract]
internal class PartyEventModifiers : AbstractEventModifiers<PartyEvent>
{
	private int _balloonCount = 5;

	[DataMember]
	[Label<Strings>("MIN_EVENT_TIME")]
	[Help<HelpText>("MIN_EVENT_TIME_TITLE", "MIN_EVENT_TIME_TEXT")]
	[Group<SideMenuGroupPartyEventModifiersGeneral>]
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
	[Group<SideMenuGroupPartyEventModifiersGeneral>]
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
	[Label<Strings>("BALLOON_COUNT")]
	[Help<HelpText>("BALLOON_COUNT_TITLE", "BALLOON_COUNT_TEXT")]
	[Group<SideMenuGroupPartyEventModifiersGeneral>]
	public virtual int BalloonCount
	{
		get
		{
			return _balloonCount;
		}
		set
		{
			_balloonCount = value;
			foreach (EventCollectorItem item in MonoBehaviourCollector<PartyEvent, EventCollectorItem>.Collection)
			{
				((PartyEvent)item.Item).balloonCount = value;
			}
		}
	}

	[Label<Strings>("EVENT_START")]
	[Help<HelpText>("EVENT_START_TITLE", "EVENT_START_TEXT")]
	[Group<SideMenuGroupPartyEventModifiersGeneral>]
	public override void StartEvent()
	{
		base.StartEvent();
	}

	[Label<Strings>("EVENT_STOP")]
	[Help<HelpText>("EVENT_STOP_TITLE", "EVENT_STOP_TEXT")]
	[Group<SideMenuGroupPartyEventModifiersGeneral>]
	public override void StopEvent()
	{
		base.StopEvent();
	}

	[Label<Strings>("CLEAN_UP_BALLOONS")]
	[Help<HelpText>("CLEAN_UP_BALLOONS_TITLE", "CLEAN_UP_BALLOONS_TEXT")]
	[Group<SideMenuGroupPartyEventModifiersGeneral>]
	public virtual void CleanBalloons()
	{
		PartyEventPatch.PartyBalloonIdentifier[] array = Resources.FindObjectsOfTypeAll<PartyEventPatch.PartyBalloonIdentifier>();
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i].gameObject);
		}
	}
}
