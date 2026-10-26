using PlusModMenu.Api.Collectors;

namespace PlusModMenu.Api.Characters;

public class CharacterCollector<TItem> : MonoBehaviourCollector<TItem, CharacterCollectorItem> where TItem : NPC
{
}
