using ModMenu.Properties;
using PlusModMenu.Api.Interfaces;

namespace ModMenu.BaldiPlusEngineGame;

internal class VanillaPlugin : IPlugin
{
	public string Guid => AssemblyInfo.PLUGIN_GUID;

	public string Name => "Vanilla";

	public string Version => AssemblyInfo.VERSION;

	public string Author => AssemblyInfo.AUTHOR;

	public string Description => "This plugin has all the default functionality of the ModMenu.";
}
