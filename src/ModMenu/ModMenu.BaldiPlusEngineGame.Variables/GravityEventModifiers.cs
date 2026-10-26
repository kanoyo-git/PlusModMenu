using System.Reflection;
using System.Runtime.Serialization;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Collectors;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Collectors;

namespace ModMenu.BaldiPlusEngineGame.Variables;

[DataContract]
internal class GravityEventModifiers : AbstractEventModifiers<GravityEvent>
{
	private static readonly FieldInfo _minFlippersField = AccessTools.Field(typeof(GravityEvent), "minFlippers");

	private static readonly FieldInfo _maxFlippersField = AccessTools.Field(typeof(GravityEvent), "maxFlippers");

	private static readonly FieldInfo _initialSpawnDelayField = AccessTools.Field(typeof(GravityEvent), "initialSpawnDelay");

	private static readonly FieldInfo _minRespawnTimeField = AccessTools.Field(typeof(GravityEvent), "minRespawnTime");

	private static readonly FieldInfo _maxRespawnTimeField = AccessTools.Field(typeof(GravityEvent), "maxRespawnTime");

	private int _minFlippers = 15;

	private int _maxFlippers = 25;

	private float _initialSpawnDelay = 10f;

	private int _minRespawnTime = 20;

	private int _maxRespawnTime = 30;

	[DataMember]
	[Label<Strings>("MIN_EVENT_TIME")]
	[Help<HelpText>("MIN_EVENT_TIME_TITLE", "MIN_EVENT_TIME_TEXT")]
	[Group<SideMenuGroupGravityEventModifiersGeneral>]
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
	[Group<SideMenuGroupGravityEventModifiersGeneral>]
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
	[Label<Strings>("MIN_FLIPPERS")]
	[Help<HelpText>("MIN_FLIPPERS_TITLE", "MIN_FLIPPERS_TEXT")]
	[Group<SideMenuGroupGravityEventModifiersGeneral>]
	public virtual int MinFlippers
	{
		get
		{
			return _minFlippers;
		}
		set
		{
			_minFlippers = value;
			foreach (EventCollectorItem item in MonoBehaviourCollector<GravityEvent, EventCollectorItem>.Collection)
			{
				_minFlippersField.SetValue(item.Item, value);
			}
		}
	}

	[DataMember]
	[Label<Strings>("MAX_FLIPPERS")]
	[Help<HelpText>("MAX_FLIPPERS_TITLE", "MAX_FLIPPERS_TEXT")]
	[Group<SideMenuGroupGravityEventModifiersGeneral>]
	public virtual int MaxFlippers
	{
		get
		{
			return _maxFlippers;
		}
		set
		{
			_maxFlippers = value;
			foreach (EventCollectorItem item in MonoBehaviourCollector<GravityEvent, EventCollectorItem>.Collection)
			{
				_maxFlippersField.SetValue(item.Item, value);
			}
		}
	}

	[DataMember]
	[Label<Strings>("INITIAL_RESPAWN_TIME")]
	[Help<HelpText>("INITIAL_RESPAWN_TIME_TITLE", "INITIAL_RESPAWN_TIME_TEXT")]
	[Group<SideMenuGroupGravityEventModifiersGeneral>]
	public virtual float InitialSpawnDelay
	{
		get
		{
			return _initialSpawnDelay;
		}
		set
		{
			_initialSpawnDelay = value;
			foreach (EventCollectorItem item in MonoBehaviourCollector<GravityEvent, EventCollectorItem>.Collection)
			{
				_initialSpawnDelayField.SetValue(item.Item, value);
			}
		}
	}

	[DataMember]
	[Label<Strings>("MIN_RESPAWN_TIME")]
	[Help<HelpText>("MIN_RESPAWN_TIME_TITLE", "MIN_RESPAWN_TIME_TEXT")]
	[Group<SideMenuGroupGravityEventModifiersGeneral>]
	[ValueRange(1, int.MaxValue)]
	public virtual int MinRespawnTime
	{
		get
		{
			return _minRespawnTime;
		}
		set
		{
			_minRespawnTime = value;
			foreach (EventCollectorItem item in MonoBehaviourCollector<GravityEvent, EventCollectorItem>.Collection)
			{
				_minRespawnTimeField.SetValue(item.Item, value);
			}
		}
	}

	[DataMember]
	[Label<Strings>("MAX_RESPAWN_TIME")]
	[Help<HelpText>("MAX_RESPAWN_TIME_TITLE", "MAX_RESPAWN_TIME_TEXT")]
	[Group<SideMenuGroupGravityEventModifiersGeneral>]
	[ValueRange(1, int.MaxValue)]
	public virtual int MaxRespawnTime
	{
		get
		{
			return _maxRespawnTime;
		}
		set
		{
			_maxRespawnTime = value;
			foreach (EventCollectorItem item in MonoBehaviourCollector<GravityEvent, EventCollectorItem>.Collection)
			{
				_maxRespawnTimeField.SetValue(item.Item, value);
			}
		}
	}

	[Label<Strings>("EVENT_START")]
	[Help<HelpText>("EVENT_START_TITLE", "EVENT_START_TEXT")]
	[Group<SideMenuGroupGravityEventModifiersGeneral>]
	public override void StartEvent()
	{
		base.StartEvent();
	}

	[Label<Strings>("EVENT_STOP")]
	[Help<HelpText>("EVENT_STOP_TITLE", "EVENT_STOP_TEXT")]
	[Group<SideMenuGroupGravityEventModifiersGeneral>]
	public override void StopEvent()
	{
		base.StopEvent();
	}
}
