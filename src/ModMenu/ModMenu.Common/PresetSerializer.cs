using System;
using System.Collections.Generic;
using System.Linq;
using ModMenu.BaldiPlusEngineGame.Services;
using ModMenu.Common.JsonConverters;
using ModMenu.Common.Serialization;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ModMenu.Common;

internal class PresetSerializer
{
	private class ReuseExistingInstanceCreationContractResolver : DefaultContractResolver
	{
		private readonly HashSet<IModifiers> _modifierInstances = ((ModMenuService)Service.Get<IModMenuService>())._modifierInstances;

		protected override JsonObjectContract CreateObjectContract(Type objectType)
		{
			JsonObjectContract objectContract = base.CreateObjectContract(objectType);
			objectContract.DefaultCreator = () => _modifierInstances.FirstOrDefault((IModifiers x) => x.GetType() == objectContract.CreatedType);
			return objectContract;
		}
	}

	private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
	{
		SerializationBinder = new JsonSerializationBinder(),
		Formatting = Formatting.None,
		TypeNameHandling = TypeNameHandling.Auto,
		TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
		ContractResolver = new ReuseExistingInstanceCreationContractResolver(),
		Converters = new List<JsonConverter>
		{
			new ColorConverter()
		},
		Error = (object _, ErrorEventArgs eventArgs) =>
		{
			eventArgs.ErrorContext.Handled = true;
		}
	};

	public static string Serialize<T>(T obj)
	{
		return JsonConvert.SerializeObject(obj, _settings);
	}

	public static T? Deserialize<T>(string json)
	{
		return JsonConvert.DeserializeObject<T>(json, _settings);
	}

	public static void Populate(string json, object target)
	{
		JsonConvert.PopulateObject(json, target, _settings);
	}
}
