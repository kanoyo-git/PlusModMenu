using System;
using System.Collections.Generic;
using System.Linq;
using ClassicYetAgain.Interfaces;
using ClassicYetAgain.Patches;
using PlusModMenu.Core.Common.Extensions;
using PlusModMenu.Api;
using PlusModMenu.Api.Characters.Interfaces;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ClassicYetAgain.BaldiClassicRemastered;

internal class LethalTouch : MonoBehaviour
{
	private class ModifierHolder
	{
		public ILethalTouch? LethalTouch { get; init; }

		public ICharacterHasPlayerTrigger? CharacterHasPlayerTrigger { get; init; }
	}

	private static readonly Dictionary<Type, ModifierHolder?> _modifierInstances = new Dictionary<Type, ModifierHolder>();

	private ModifierHolder _modifiers;

	private NPC _npc;

	public void SetValues(Type? npcType, NPC npc)
	{
		if (npcType == null)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		if (!_modifierInstances.TryGetValue(npcType, out ModifierHolder value))
		{
			Type characterType = typeof(IModifiersForCharacter<>).MakeGenericType(npcType);
			IModifiers[] source = Service.Get<IModMenuService>().Modifiers.Where((IModifiers x) => x.GetType().IsAssignableTo(characterType)).ToArray();
			ModifierHolder value2 = new ModifierHolder
			{
				LethalTouch = (source.FirstOrDefault((IModifiers x) => x is ILethalTouch) as ILethalTouch),
				CharacterHasPlayerTrigger = (source.FirstOrDefault((IModifiers x) => x is ICharacterHasPlayerTrigger) as ICharacterHasPlayerTrigger)
			};
			_modifierInstances.Add(npcType, value2);
			value = _modifierInstances[npcType];
		}
		if (value == null)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		_modifiers = value;
		_npc = npc;
	}

	public void OnTriggerEnter(Collider other)
	{
		if (!enabled || !other.CompareTag("Player") || other.GetComponent<PlayerManager>() is { invincible: true })
		{
			return;
		}
		ILethalTouch lethalTouch = _modifiers.LethalTouch;
		if (lethalTouch != null && lethalTouch.LethalTouch)
		{
			ICharacterHasPlayerTrigger characterHasPlayerTrigger = _modifiers.CharacterHasPlayerTrigger;
			if (characterHasPlayerTrigger == null || !characterHasPlayerTrigger.IgnoresPlayerTrigger)
			{
				CoreGameManagerPatch.Helper.EndGame(other.transform, _npc);
			}
		}
	}

	public void ForceEnd(Collider other)
	{
		CoreGameManagerPatch.Helper.EndGame(other.transform, _npc);
	}
}
