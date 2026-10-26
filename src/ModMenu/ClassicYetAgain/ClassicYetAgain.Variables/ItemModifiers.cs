using System.Linq;
using System.Runtime.Serialization;
using ClassicYetAgain.Collectors;
using PlusModMenu.Core.Common;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Collectors;
using PlusModMenu.Api.Interfaces;
using UnityEngine;

namespace ClassicYetAgain.Variables;

[DataContract]
public class ItemModifiers : IModifiers
{
	[Label<Strings>("RANDOMIZE_ITEM_POSITIONS")]
	[Help<HelpText>("RANDOMIZE_ITEM_POSITIONS_TITLE", "RANDOMIZE_ITEM_POSITIONS_TEXT")]
	[Group<ItemModifiersGeneralGroup>]
	public virtual void RandomizeItemPositions()
	{
		ItemCollectorItem[] array = MonoBehaviourCollector<Pickup, ItemCollectorItem>.Collection.ToArray();
		Randomizer.Array(array);
		for (int i = 0; i < MonoBehaviourCollector<Pickup, ItemCollectorItem>.Collection.Count; i++)
		{
			ItemCollectorItem itemCollectorItem = MonoBehaviourCollector<Pickup, ItemCollectorItem>.Collection[i];
			Transform transform = itemCollectorItem.Item.transform;
			Transform transform2 = array[i].Item.transform;
			Vector3 position = array[i].Item.transform.position;
			Vector3 position2 = itemCollectorItem.Item.transform.position;
			transform.position = position;
			transform2.position = position2;
		}
	}
}
