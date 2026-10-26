using System;
using System.Runtime.Serialization;

namespace PlusModMenu.Core.Common;

[DataContract]
public class Serializable<T>
{
	[OnDeserializing]
	private void OnDeserializing(StreamingContext context)
	{
		typeof(T).GetConstructor(new Type[0])?.Invoke(this, null);
	}
}
