using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PlusModMenu.Core.Common.Extensions;
using PlusModMenu.Api;
using PlusModMenu.Api.Characters.Interfaces;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.Services;

internal class CharacterHelperService : ICharacterHelperService, IService
{
	private static readonly Dictionary<Type, IEnumerable<IModifiers>> _modifiers = new Dictionary<Type, IEnumerable<IModifiers>>();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IEnumerable<IModifiers> GetCharacterModifiersOfNPC<TNPC>() where TNPC : NPC
	{
		return GetCharacterModifiersOfNPC(typeof(TNPC));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IEnumerable<IModifiers> GetCharacterModifiersOfNPC(Type npcType)
	{
		Type characterModifiersType = typeof(IModifiersForCharacter<>).MakeGenericType(npcType);
		if (!_modifiers.TryGetValue(characterModifiersType, out IEnumerable<IModifiers> value))
		{
			value = (_modifiers[characterModifiersType] = Service.Get<IModMenuService>().Modifiers.Where((IModifiers x) => x.GetType().IsAssignableTo(characterModifiersType)));
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TModifiers? GetSpecificCharacterModifiersOfNPC<TModifiers, TNPC>() where TModifiers : IModifiers where TNPC : NPC
	{
		IModifiers specificCharacterModifiersOfNPC = GetSpecificCharacterModifiersOfNPC(typeof(TModifiers), typeof(TNPC));
		if (specificCharacterModifiersOfNPC is TModifiers)
		{
			return (TModifiers)specificCharacterModifiersOfNPC;
		}
		return default;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IModifiers? GetSpecificCharacterModifiersOfNPC(Type characterModifiersType, Type npcType)
	{
		return GetCharacterModifiersOfNPC(npcType).FirstOrDefault((IModifiers x) => x.GetType().IsAssignableTo(characterModifiersType));
	}
}
