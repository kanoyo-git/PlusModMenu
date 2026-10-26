namespace PlusModMenu.Api.Interfaces;

public interface IPlugin
{
	string Guid { get; }

	string Name { get; }

	string Version { get; }

	string Author { get; }

	string Description { get; }
}
