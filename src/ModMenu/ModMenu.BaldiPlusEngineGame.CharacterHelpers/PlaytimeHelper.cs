using System.Reflection;
using PlusModMenu.Core.Common.Extensions;
using HarmonyLib;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.CharacterHelpers;

internal class PlaytimeHelper : MonoBehaviour
{
	private static readonly FieldInfo _currentJumprope = AccessTools.Field(typeof(Playtime), "currentJumprope");

	private static readonly FieldInfo _moveMod = AccessTools.Field(typeof(Jumprope), "moveMod");

	private static readonly FieldInfo _textCanvas = AccessTools.Field(typeof(Jumprope), "textCanvas");

	private static readonly FieldInfo _ropeCanvas = AccessTools.Field(typeof(Jumprope), "ropeCanvas");

	private Playtime _playtime;

	private void Awake()
	{
		_playtime = GetComponent<Playtime>();
	}

	private void OnEnable()
	{
		Jumprope value = _currentJumprope.GetValue<Jumprope>(_playtime);
		if (value != null)
		{
			MovementModifier value2 = _moveMod.GetValue<MovementModifier>(value);
			value.player.Am.moveMods.Add(value2);
			if (!value.player.jumpropes.Contains(value))
			{
				value.player.jumpropes.Add(value);
			}
			Singleton<CoreGameManager>.Instance.GetCamera(value.player.playerNumber).UpdateTargets(value.transform, 24);
			_textCanvas.GetValue<Canvas>(value).gameObject.SetActive(value: true);
			_ropeCanvas.GetValue<Canvas>(value).gameObject.SetActive(value: true);
			value.gameObject.SetActive(value: true);
		}
	}

	private void OnDisable()
	{
		Jumprope value = _currentJumprope.GetValue<Jumprope>(_playtime);
		if (value != null)
		{
			MovementModifier value2 = _moveMod.GetValue<MovementModifier>(value);
			value.player.Am.moveMods.Remove(value2);
			value.player.jumpropes.Remove(value);
			Singleton<CoreGameManager>.Instance.GetCamera(value.player.playerNumber).UpdateTargets(null, 24);
			_textCanvas.GetValue<Canvas>(value).gameObject.SetActive(value: false);
			_ropeCanvas.GetValue<Canvas>(value).gameObject.SetActive(value: false);
			value.gameObject.SetActive(value: false);
		}
	}
}
