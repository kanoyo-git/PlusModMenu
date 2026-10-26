using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModMenu.BaldiPlusEngineGame.Services;
using PlusModMenu.Api;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ModMenu.Common.Serialization;

internal class DefaultValueResolver : DefaultContractResolver
{
	private readonly HashSet<IModifiers> _modifierInstances;

	public DefaultValueResolver()
	{
		_modifierInstances = ((ModMenuService)Service.Get<IModMenuService>())._modifierInstances;
	}

	protected override JsonObjectContract CreateObjectContract(Type objectType)
	{
		JsonObjectContract objectContract = base.CreateObjectContract(objectType);
		objectContract.DefaultCreator = () => _modifierInstances.FirstOrDefault((IModifiers x) => x.GetType() == objectContract.CreatedType);
		return objectContract;
	}

	protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
	{
		JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);
		if (jsonProperty.AttributeProvider?.GetAttributes(typeof(NoDefaultAttribute), inherit: true).FirstOrDefault() is NoDefaultAttribute)
		{
			jsonProperty.Ignored = true;
		}
		return jsonProperty;
	}
}
