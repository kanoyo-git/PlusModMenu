using System;
using System.Collections.Generic;

namespace PlusModMenu.Api.Interfaces.Services;

internal interface ICharacterHelperService : IService
{
	IEnumerable<IModifiers> GetCharacterModifiersOfNPC<TNPC>() where TNPC : NPC;

	IEnumerable<IModifiers> GetCharacterModifiersOfNPC(Type npcType);

	TModifiers? GetSpecificCharacterModifiersOfNPC<TModifiers, TNPC>() where TModifiers : IModifiers where TNPC : NPC;

	IModifiers? GetSpecificCharacterModifiersOfNPC(Type characterModifiersType, Type npcType);
}
