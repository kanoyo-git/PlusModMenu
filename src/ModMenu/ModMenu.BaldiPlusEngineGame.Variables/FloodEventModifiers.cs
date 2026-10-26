using System.Reflection;
using System.Runtime.Serialization;
using PlusModMenu.Core.Common.Extensions;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Collectors;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Collectors;

namespace ModMenu.BaldiPlusEngineGame.Variables;

[DataContract]
internal class FloodEventModifiers : AbstractEventModifiers<FloodEvent>
{
	private static readonly FieldInfo _heightField = AccessTools.Field(typeof(FloodEvent), "height");

	private static readonly FieldInfo _riseSpeedField = AccessTools.Field(typeof(FloodEvent), "riseSpeed");

	private static readonly FieldInfo _minWhirlpoolsField = AccessTools.Field(typeof(FloodEvent), "minWhirlpools");

	private static readonly FieldInfo _maxWhirlpoolsField = AccessTools.Field(typeof(FloodEvent), "maxWhirlpools");

	private static readonly FieldInfo _moveModField = AccessTools.Field(typeof(FloodEvent), "moveMod");

	private float _maxHeight = 3f;

	private float _riseSpeed = 1f;

	private int _minWhirlpools = 15;

	private int _maxWhirlpools = 30;

	private float _movementMultiplier = 0.75f;

	[DataMember]
	[Label<Strings>("MIN_EVENT_TIME")]
	[Help<HelpText>("MIN_EVENT_TIME_TITLE", "MIN_EVENT_TIME_TEXT")]
	[Group<SideMenuGroupFloodEventModifiersGeneral>]
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
	[Group<SideMenuGroupFloodEventModifiersGeneral>]
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
	[Label<Strings>("MAX_HEIGHT")]
	[Help<HelpText>("MAX_HEIGHT_TITLE", "MAX_HEIGHT_TEXT")]
	[Group<SideMenuGroupFloodEventModifiersGeneral>]
	public virtual float MaxHeight
	{
		get
		{
			return _maxHeight;
		}
		set
		{
			_maxHeight = value;
			foreach (EventCollectorItem item in MonoBehaviourCollector<FloodEvent, EventCollectorItem>.Collection)
			{
				_heightField.SetValue(item.Item, value);
			}
		}
	}

	[DataMember]
	[Label<Strings>("RISING_SPEED")]
	[Help<HelpText>("RISING_SPEED_TITLE", "RISING_SPEED_TEXT")]
	[Group<SideMenuGroupFloodEventModifiersGeneral>]
	public virtual float RiseSpeed
	{
		get
		{
			return _riseSpeed;
		}
		set
		{
			_riseSpeed = value;
			foreach (EventCollectorItem item in MonoBehaviourCollector<FloodEvent, EventCollectorItem>.Collection)
			{
				_riseSpeedField.SetValue(item.Item, value);
			}
		}
	}

	[DataMember]
	[Label<Strings>("MIN_WHIRLPOOLS")]
	[Help<HelpText>("MIN_WHIRLPOOLS_TITLE", "MIN_WHIRLPOOLS_TEXT")]
	[Group<SideMenuGroupFloodEventModifiersGeneral>]
	public virtual int MinWhirlpools
	{
		get
		{
			return _minWhirlpools;
		}
		set
		{
			_minWhirlpools = value;
			foreach (EventCollectorItem item in MonoBehaviourCollector<FloodEvent, EventCollectorItem>.Collection)
			{
				_minWhirlpoolsField.SetValue(item.Item, value);
			}
		}
	}

	[DataMember]
	[Label<Strings>("MAX_WHIRLPOOLS")]
	[Help<HelpText>("MAX_WHIRLPOOLS_TITLE", "MAX_WHIRLPOOLS_TEXT")]
	[Group<SideMenuGroupFloodEventModifiersGeneral>]
	public virtual int MaxWhirlpools
	{
		get
		{
			return _maxWhirlpools;
		}
		set
		{
			_maxWhirlpools = value;
			foreach (EventCollectorItem item in MonoBehaviourCollector<FloodEvent, EventCollectorItem>.Collection)
			{
				_maxWhirlpoolsField.SetValue(item.Item, value);
			}
		}
	}

	[DataMember]
	[Label<Strings>("MOVEMENT_MULTIPLIER")]
	[Help<HelpText>("MOVEMENT_MULTIPLIER_TITLE", "MOVEMENT_MULTIPLIER_TEXT")]
	[Group<SideMenuGroupFloodEventModifiersGeneral>]
	public virtual float MovementMultiplier
	{
		get
		{
			return _movementMultiplier;
		}
		set
		{
			_movementMultiplier = value;
			foreach (EventCollectorItem item in MonoBehaviourCollector<FloodEvent, EventCollectorItem>.Collection)
			{
				_moveModField.GetValue<MovementModifier>(item.Item).movementMultiplier = value;
			}
		}
	}

	[Label<Strings>("EVENT_START")]
	[Help<HelpText>("EVENT_START_TITLE", "EVENT_START_TEXT")]
	[Group<SideMenuGroupFloodEventModifiersGeneral>]
	public override void StartEvent()
	{
		base.StartEvent();
	}

	[Label<Strings>("EVENT_STOP")]
	[Help<HelpText>("EVENT_STOP_TITLE", "EVENT_STOP_TEXT")]
	[Group<SideMenuGroupFloodEventModifiersGeneral>]
	public override void StopEvent()
	{
		base.StopEvent();
	}
}
