using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ClassicYetAgain.Variables.Menus;
using PlusModMenu.Core.Common;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Characters;
using PlusModMenu.Api.Collectors;
using PlusModMenu.Api.Interfaces;
using UnityEngine;

namespace ClassicYetAgain.Variables.CharacterModifiers;

[DataContract]
public class CharacterModifiers : IModifiers
{
	[Label<Strings>("RANDOMIZE_CHARACTER_POSITIONS")]
	[Help<HelpText>("RANDOMIZE_CHARACTER_POSITIONS_TITLE", "RANDOMIZE_CHARACTER_POSITIONS_TEXT")]
	[Group<CharacterModifiersGeneralGeneralGroup>]
	public virtual void RandomizeCharacterPositions()
	{
		IReadOnlyList<CharacterCollectorItem> collection = MonoBehaviourCollector<NPC, CharacterCollectorItem>.Collection;
		CharacterCollectorItem[] array = collection.ToArray();
		Randomizer.Array(array);
		for (int i = 0; i < collection.Count; i++)
		{
			CharacterCollectorItem characterCollectorItem = collection[i];
			CharacterCollectorItem characterCollectorItem2 = array[i];
			Vector3 position = characterCollectorItem.Item.transform.position;
			Vector3 position2 = characterCollectorItem2.Item.transform.position;
			SetPosition(characterCollectorItem.Item, new Vector3(position2.x, position.y, position2.z));
			SetPosition(characterCollectorItem2.Item, new Vector3(position.x, position2.y, position.z));
		}
		static void SetPosition(NPC npc, Vector3 position)
		{
			if (npc.Entity != null)
			{
				npc.Entity.Teleport(position);
			}
			else
			{
				npc.transform.position = position;
			}
		}
	}
}
