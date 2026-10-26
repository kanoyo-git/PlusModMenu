using System.Reflection;
using PlusModMenu.Core.Common.Extensions;
using HarmonyLib;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.CharacterHelpers;

internal class MrsPompHelper : MonoBehaviour
{
	private static readonly FieldInfo _targetedPlayer = AccessTools.Field(typeof(NoLateTeacher), "targetedPlayer");

	private static readonly FieldInfo _moveMod = AccessTools.Field(typeof(NoLateTeacher), "moveMod");

	private NoLateTeacher _mrsPomp;

	private MovementModifier _moveModInstance;

	private void Awake()
	{
		_mrsPomp = GetComponent<NoLateTeacher>();
		_moveModInstance = _moveMod.GetValue<MovementModifier>(_mrsPomp);
	}

	private void OnEnable()
	{
		PlayerManager value = _targetedPlayer.GetValue<PlayerManager>(_mrsPomp);
		if (value != null)
		{
			value.Am.moveMods.Add(_moveModInstance);
		}
	}

	private void OnDisable()
	{
		PlayerManager value = _targetedPlayer.GetValue<PlayerManager>(_mrsPomp);
		if (value != null)
		{
			value.Am.moveMods.Remove(_moveModInstance);
		}
	}
}
