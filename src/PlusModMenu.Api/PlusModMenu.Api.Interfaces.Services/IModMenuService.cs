using System;
using System.Collections.Generic;

namespace PlusModMenu.Api.Interfaces.Services;

public interface IModMenuService : IService
{
	IReadOnlyCollection<IModifiers> Modifiers { get; }

	TModifiers? Get<TModifiers>() where TModifiers : class, IModifiers;

	object? Get(Type? modifiersType);
}
