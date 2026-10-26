using PlusModMenu.Api.Collectors;

namespace ModMenu.BaldiPlusEngineGame.Collectors;

public class EventCollector<TEvent> : MonoBehaviourCollector<TEvent, EventCollectorItem> where TEvent : RandomEvent
{
}
