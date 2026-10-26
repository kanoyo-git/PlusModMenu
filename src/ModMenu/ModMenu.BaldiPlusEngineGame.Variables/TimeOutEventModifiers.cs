using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Collectors;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Collectors;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables;

[DataContract]
internal class TimeOutEventModifiers : AbstractEventModifiers<TimeOut>
{
	private float _baldiAngerRate = 1f;

	private float _baldiAngerAmount = 0.1f;

	private float _lightOffRate = 5f;

	[DataMember]
	[Label<Strings>("MIN_EVENT_TIME")]
	[Help<HelpText>("MIN_EVENT_TIME_TITLE", "MIN_EVENT_TIME_TEXT")]
	[Group<SideMenuGroupTimeOutEventModifiersGeneral>]
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
	[Group<SideMenuGroupTimeOutEventModifiersGeneral>]
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
	[Label<Strings>("TIME_OUT_ANGER_RATE")]
	[Help<HelpText>("TIME_OUT_ANGER_RATE_TITLE", "TIME_OUT_ANGER_RATE_TEXT")]
	[Group<SideMenuGroupTimeOutEventModifiersGeneral>]
	public virtual float BaldiAngerRate
	{
		get
		{
			return _baldiAngerRate;
		}
		set
		{
			_baldiAngerRate = value;
			ApplyToAll();
		}
	}

	[DataMember]
	[Label<Strings>("TIME_OUT_ANGER_AMOUNT")]
	[Help<HelpText>("TIME_OUT_ANGER_AMOUNT_TITLE", "TIME_OUT_ANGER_AMOUNT_TEXT")]
	[Group<SideMenuGroupTimeOutEventModifiersGeneral>]
	public virtual float BaldiAngerAmount
	{
		get
		{
			return _baldiAngerAmount;
		}
		set
		{
			_baldiAngerAmount = value;
			ApplyToAll();
		}
	}

	[DataMember]
	[Label<Strings>("TIME_OUT_LIGHT_RATE")]
	[Help<HelpText>("TIME_OUT_LIGHT_RATE_TITLE", "TIME_OUT_LIGHT_RATE_TEXT")]
	[Group<SideMenuGroupTimeOutEventModifiersGeneral>]
	public virtual float LightOffRate
	{
		get
		{
			return _lightOffRate;
		}
		set
		{
			_lightOffRate = value;
			ApplyToAll();
		}
	}

	[Label<Strings>("EVENT_START")]
	[Help<HelpText>("EVENT_START_TITLE", "TIME_OUT_START_TEXT")]
	[Group<SideMenuGroupTimeOutEventModifiersGeneral>]
	public override void StartEvent()
	{
		base.StartEvent();
	}

	internal override void ApplyTo(RandomEvent randomEvent)
	{
		if (randomEvent is not TimeOut)
		{
			return;
		}
		Set(randomEvent, "baldiAngerRate", _baldiAngerRate);
		Set(randomEvent, "baldiAngerAmount", _baldiAngerAmount);
		Set(randomEvent, "lightOffRate", _lightOffRate);
	}

	[HarmonyLib.HarmonyPatch(typeof(TimeOut), "Update")]
	private class UpdatePatch
	{
		private static readonly System.Reflection.FieldInfo _ec = HarmonyLib.AccessTools.Field(typeof(RandomEvent), "ec");

		private static readonly System.Reflection.FieldInfo _timeToNextAnger = HarmonyLib.AccessTools.Field(typeof(TimeOut), "timeToNextAnger");

		private static readonly System.Reflection.FieldInfo _baldiAngerRate = HarmonyLib.AccessTools.Field(typeof(TimeOut), "baldiAngerRate");

		private static void Prefix(TimeOut __instance)
		{
			if (__instance.Active && _ec.GetValue(__instance) is EnvironmentController ec && ec.GetBaldi() == null)
			{
				_timeToNextAnger.SetValue(__instance, (float)_baldiAngerRate.GetValue(__instance) + 1f);
			}
		}
	}
}
