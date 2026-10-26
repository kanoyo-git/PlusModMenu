using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Collectors;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Collectors;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables;

[DataContract]
internal class BalderDashEventModifiers : AbstractEventModifiers<BalderEvent>
{
	private float _spawnRate = 5f;

	[DataMember]
	[Label<Strings>("MIN_EVENT_TIME")]
	[Help<HelpText>("MIN_EVENT_TIME_TITLE", "MIN_EVENT_TIME_TEXT")]
	[Group<SideMenuGroupBalderDashEventModifiersGeneral>]
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
	[Group<SideMenuGroupBalderDashEventModifiersGeneral>]
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
	[Label<Strings>("BALDER_SPAWN_RATE")]
	[Help<HelpText>("BALDER_SPAWN_RATE_TITLE", "BALDER_SPAWN_RATE_TEXT")]
	[Group<SideMenuGroupBalderDashEventModifiersGeneral>]
	public virtual float SpawnRate
	{
		get
		{
			return _spawnRate;
		}
		set
		{
			_spawnRate = value;
			ApplyToAll();
		}
	}

	[Label<Strings>("EVENT_START")]
	[Help<HelpText>("EVENT_START_TITLE", "EVENT_START_TEXT")]
	[Group<SideMenuGroupBalderDashEventModifiersGeneral>]
	public override void StartEvent()
	{
		EnsureSpawnRoom();
		base.StartEvent();
	}

	[Label<Strings>("EVENT_STOP")]
	[Help<HelpText>("EVENT_STOP_TITLE", "EVENT_STOP_TEXT")]
	[Group<SideMenuGroupBalderDashEventModifiersGeneral>]
	public override void StopEvent()
	{
		base.StopEvent();
	}

	internal override void ApplyTo(RandomEvent randomEvent)
	{
		if (randomEvent is not BalderEvent)
		{
			return;
		}
		Set(randomEvent, "spawnRate", _spawnRate);
	}
}
