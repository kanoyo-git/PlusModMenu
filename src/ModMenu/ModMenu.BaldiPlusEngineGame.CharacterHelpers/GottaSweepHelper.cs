using System.Collections.Generic;
using System.Reflection;
using PlusModMenu.Core.Common.Extensions;
using HarmonyLib;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.CharacterHelpers;

internal class GottaSweepHelper : MonoBehaviour
{
	internal class GottaSweepTriggerFix : MonoBehaviour
	{
		private GottaSweep _gottaSweep;

		public void Initialize(GottaSweep gottaSweep)
		{
			_gottaSweep = gottaSweep;
		}

		private void OnDisable()
		{
			if (_gottaSweep != null)
			{
				Entity component = GetComponent<Entity>();
				if (component != null)
				{
					Release(_gottaSweep, component.ExternalActivity);
				}
			}
		}
	}

	private static readonly FieldInfo _actMods = AccessTools.Field(typeof(GottaSweep), "actMods");

	private static readonly FieldInfo _moveMod = AccessTools.Field(typeof(GottaSweep), "moveMod");

	private GottaSweep _gottaSweep;

	private IList<ActivityModifier> _actModsList;

	private void Awake()
	{
		_gottaSweep = GetComponent<GottaSweep>();
		_actModsList = _actMods.GetValue<IList<ActivityModifier>>(_gottaSweep);
	}

	private void OnDisable()
	{
		ActivityModifier[] array = new ActivityModifier[_actModsList.Count];
		_actModsList.CopyTo(array, 0);
		for (int i = 0; i < array.Length; i++)
		{
			Release(_gottaSweep, array[i]);
		}
	}

	private static void Release(GottaSweep gottaSweep, ActivityModifier activityModifier)
	{
		if (!(activityModifier == null))
		{
			activityModifier.moveMods.Remove(_moveMod.GetValue<MovementModifier>(gottaSweep));
			_actMods.GetValue<IList<ActivityModifier>>(gottaSweep).Remove(activityModifier);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((object)other != null && other.tag == "NPC" && other.isTrigger)
		{
			other.gameObject.AddComponent<GottaSweepTriggerFix>().Initialize(_gottaSweep);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((object)other != null && other.tag == "NPC" && other.isTrigger)
		{
			Object.Destroy(other.GetComponent<GottaSweepTriggerFix>());
		}
	}
}
