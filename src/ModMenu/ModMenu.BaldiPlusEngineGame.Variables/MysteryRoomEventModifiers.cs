using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Collectors;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Collectors;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables;

[DataContract]
internal class MysteryRoomEventModifiers : AbstractEventModifiers<MysteryRoom>
{
	private static readonly System.Reflection.FieldInfo _room = HarmonyLib.AccessTools.Field(typeof(RandomEvent), "room");

	[DataMember]
	[Label<Strings>("MIN_EVENT_TIME")]
	[Help<HelpText>("MIN_EVENT_TIME_TITLE", "MIN_EVENT_TIME_TEXT")]
	[Group<SideMenuGroupMysteryRoomEventModifiersGeneral>]
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
	[Group<SideMenuGroupMysteryRoomEventModifiersGeneral>]
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

	[Label<Strings>("EVENT_START")]
	[Help<HelpText>("EVENT_START_TITLE", "MYSTERY_ROOM_START_TEXT")]
	[Group<SideMenuGroupMysteryRoomEventModifiersGeneral>]
	public override void StartEvent()
	{
		foreach (EventCollectorItem item in MonoBehaviourCollector<MysteryRoom, EventCollectorItem>.Collection)
		{
			if (!item.Item.Active && _room.GetValue(item.Item) != null)
			{
				item.Item.SetEventTime(new System.Random());
				item.Item.Begin();
				return;
			}
		}
	}

	[Label<Strings>("EVENT_STOP")]
	[Help<HelpText>("EVENT_STOP_TITLE", "EVENT_STOP_TEXT")]
	[Group<SideMenuGroupMysteryRoomEventModifiersGeneral>]
	public override void StopEvent()
	{
		base.StopEvent();
	}
}
