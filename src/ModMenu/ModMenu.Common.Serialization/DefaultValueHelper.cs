using System.Collections.Generic;
using ModMenu.Common.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ModMenu.Common.Serialization;

internal class DefaultValueHelper
{
	public static JsonSerializerSettings Settings => new JsonSerializerSettings
	{
		SerializationBinder = new JsonSerializationBinder(),
		Formatting = Formatting.None,
		TypeNameHandling = TypeNameHandling.Auto,
		TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
		ContractResolver = new DefaultValueResolver(),
		Converters = new List<JsonConverter>
		{
			new ColorConverter()
		},
		Error = (object _, ErrorEventArgs eventArgs) =>
		{
			eventArgs.ErrorContext.Handled = true;
		}
	};
}
