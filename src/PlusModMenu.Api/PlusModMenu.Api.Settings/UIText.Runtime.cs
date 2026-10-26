using PlusModMenu.Core.Common;

namespace PlusModMenu.Api.Settings;

public readonly partial struct UIText
{
	public static System.Func<System.Type, string, string> DefaultLookup { get; set; } = (type, key) => ResourceHelper.GetString(type, key);

	public string Resolve()
	{
		return Resolve(DefaultLookup);
	}
}
