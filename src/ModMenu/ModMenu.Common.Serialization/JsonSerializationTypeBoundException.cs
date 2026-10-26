using System;
using Newtonsoft.Json;

namespace ModMenu.Common.Serialization;

internal class JsonSerializationTypeBoundException : JsonSerializationException
{
	public JsonSerializationTypeBoundException(Exception innerException)
		: base(innerException.Message, innerException)
	{
	}
}
