using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Collectors;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Collectors;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables;

[DataContract]
internal class FogEventModifiers : AbstractEventModifiers<FogEvent>
{
	private Color _fogColor = Color.gray;

	private float _fogStrength = 1f;

	private float _startDist = 5f;

	private float _maxDist = 100f;

	[DataMember]
	[Label<Strings>("MIN_EVENT_TIME")]
	[Help<HelpText>("MIN_EVENT_TIME_TITLE", "MIN_EVENT_TIME_TEXT")]
	[Group<SideMenuGroupFogEventModifiersGeneral>]
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
	[Group<SideMenuGroupFogEventModifiersGeneral>]
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
	[Label<Strings>("FOG_COLOR")]
	[Help<HelpText>("FOG_COLOR_TITLE", "FOG_COLOR_TEXT")]
	[Group<SideMenuGroupFogEventModifiersGeneral>]
	public virtual Color FogColor
	{
		get
		{
			return _fogColor;
		}
		set
		{
			_fogColor = value;
			foreach (EventCollectorItem item in MonoBehaviourCollector<FogEvent, EventCollectorItem>.Collection)
			{
				((FogEvent)item.Item).fog.color = value;
			}
			if (!(Singleton<BaseGameManager>.Instance == null))
			{
				Singleton<BaseGameManager>.Instance.Ec.UpdateFog();
			}
		}
	}

	[DataMember]
	[Label<Strings>("FOG_STRENGTH")]
	[Help<HelpText>("FOG_STRENGTH_TITLE", "FOG_STRENGTH_TEXT")]
	[Group<SideMenuGroupFogEventModifiersGeneral>]
	public virtual float FogStrength
	{
		get
		{
			return _fogStrength;
		}
		set
		{
			_fogStrength = value;
			foreach (EventCollectorItem item in MonoBehaviourCollector<FogEvent, EventCollectorItem>.Collection)
			{
				((FogEvent)item.Item).fog.strength = value;
			}
			if (!(Singleton<BaseGameManager>.Instance == null))
			{
				Singleton<BaseGameManager>.Instance.Ec.UpdateFog();
			}
		}
	}

	[DataMember]
	[Label<Strings>("FOG_START_DIST")]
	[Help<HelpText>("FOG_START_DIST_TITLE", "FOG_START_DIST_TEXT")]
	[Group<SideMenuGroupFogEventModifiersGeneral>]
	public virtual float StartDist
	{
		get
		{
			return _startDist;
		}
		set
		{
			_startDist = value;
			foreach (EventCollectorItem item in MonoBehaviourCollector<FogEvent, EventCollectorItem>.Collection)
			{
				((FogEvent)item.Item).fog.startDist = value;
			}
			if (!(Singleton<BaseGameManager>.Instance == null))
			{
				Singleton<BaseGameManager>.Instance.Ec.UpdateFog();
			}
		}
	}

	[DataMember]
	[Label<Strings>("FOG_MAX_DIST")]
	[Help<HelpText>("FOG_MAX_DIST_TITLE", "FOG_MAX_DIST_TEXT")]
	[Group<SideMenuGroupFogEventModifiersGeneral>]
	public virtual float MaxDist
	{
		get
		{
			return _maxDist;
		}
		set
		{
			_maxDist = value;
			foreach (EventCollectorItem item in MonoBehaviourCollector<FogEvent, EventCollectorItem>.Collection)
			{
				((FogEvent)item.Item).fog.maxDist = value;
			}
			if (!(Singleton<BaseGameManager>.Instance == null))
			{
				Singleton<BaseGameManager>.Instance.Ec.UpdateFog();
			}
		}
	}

	[Label<Strings>("EVENT_START")]
	[Help<HelpText>("EVENT_START_TITLE", "EVENT_START_TEXT")]
	[Group<SideMenuGroupFogEventModifiersGeneral>]
	public override void StartEvent()
	{
		base.StartEvent();
	}

	[Label<Strings>("EVENT_STOP")]
	[Help<HelpText>("EVENT_STOP_TITLE", "EVENT_STOP_TEXT")]
	[Group<SideMenuGroupFogEventModifiersGeneral>]
	public override void StopEvent()
	{
		base.StopEvent();
	}
}
