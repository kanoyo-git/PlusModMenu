using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using PlusModMenu.Core.Common;
using PlusModMenu.Core.Unity;
using ClassicYetAgain.Variables.Menus;
using PlusModMenu.Api;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Characters;
using PlusModMenu.Api.Collectors;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ClassicYetAgain.Variables;

[DataContract]
public class GameplayModifiers : IModifiers
{
	private readonly CameraModifier _cameraModifier = new CameraModifier(Vector3.zero, Vector3.zero);

	private readonly TaskManager.Task _earthquakeTask;

	private Dictionary<NPC, NPC> _shuffledNpcs = new Dictionary<NPC, NPC>();

	private bool _earthquake;

	private bool _deception;

	[DataMember]
	[Label<Strings>("EARTHQUAKE")]
	[Help<HelpText>("EARTHQUAKE_TITLE", "EARTHQUAKE_TEXT")]
	[Group<GameplayModifiersOtherGroup>]
	public virtual bool Earthquake
	{
		get
		{
			return _earthquake;
		}
		set
		{
			_earthquake = value;
			if (value)
			{
				_earthquakeTask.Start(resumeIfAlreadyRunning: true);
				return;
			}
			_earthquakeTask.Pause();
			_cameraModifier.posOffset = Vector3.zero;
		}
	}

	[DataMember]
	[Label<Strings>("EARTHQUAKE_STRENGTH")]
	[Help<HelpText>("EARTHQUAKE_STRENGTH_TITLE", "EARTHQUAKE_STRENGTH_TEXT")]
	[Group<GameplayModifiersOtherGroup>]
	public virtual float EarthquakeStrength { get; set; } = 1f;

	[DataMember]
	[Label<Strings>("DECEPTION")]
	[Help<HelpText>("DECEPTION_TITLE", "DECEPTION_TEXT")]
	[Group<GameplayModifiersOtherGroup>]
	public virtual bool Deception
	{
		get
		{
			return _deception;
		}
		set
		{
			_deception = value;
			if (value)
			{
				CharacterCollectorItem[] array = MonoBehaviourCollector<NPC, CharacterCollectorItem>.Collection.ToArray();
				Randomizer.Array(array);
				_shuffledNpcs = MonoBehaviourCollector<NPC, CharacterCollectorItem>.Collection.Zip(array, (CharacterCollectorItem k, CharacterCollectorItem v) => new { k, v }).ToDictionary(x => x.k.Item, x => x.v.Item);
				{
					foreach (KeyValuePair<NPC, NPC> shuffledNpc in _shuffledNpcs)
					{
						NPC key = shuffledNpc.Key;
						NPC value2 = shuffledNpc.Value;
						Transform transform = key.spriteRenderer[0].transform;
						Transform transform2 = value2.spriteRenderer[0].transform;
						Vector3 position = value2.spriteRenderer[0].transform.position;
						Vector3 position2 = key.spriteRenderer[0].transform.position;
						transform.position = position;
						transform2.position = position2;
						transform2 = key.spriteRenderer[0].transform;
						transform = value2.spriteRenderer[0].transform;
						Transform parent = value2.spriteRenderer[0].transform.parent;
						Transform parent2 = key.spriteRenderer[0].transform.parent;
						transform2.parent = parent;
						transform.parent = parent2;
					}
					return;
				}
			}
			for (int num = _shuffledNpcs.Count - 1; num >= 0; num--)
			{
				KeyValuePair<NPC, NPC> keyValuePair = _shuffledNpcs.ElementAt(num);
				NPC key2 = keyValuePair.Key;
				NPC value3 = keyValuePair.Value;
				Transform transform = key2.spriteRenderer[0].transform;
				Transform transform2 = value3.spriteRenderer[0].transform;
				Vector3 position2 = value3.spriteRenderer[0].transform.position;
				Vector3 position = key2.spriteRenderer[0].transform.position;
				transform.position = position2;
				transform2.position = position;
				transform2 = key2.spriteRenderer[0].transform;
				transform = value3.spriteRenderer[0].transform;
				Transform parent2 = value3.spriteRenderer[0].transform.parent;
				Transform parent = key2.spriteRenderer[0].transform.parent;
				transform2.parent = parent2;
				transform.parent = parent;
			}
			_shuffledNpcs.Clear();
		}
	}

	protected GameplayModifiers()
	{
		Service.Get<IEventService>().Subscribe<IPlayerInstantiatedEvent>((object? _) =>
		{
			Singleton<CoreGameManager>.Instance.GetCamera(0).cameraModifiers.Add(_cameraModifier);
			Singleton<CoreGameManager>.Instance.GetPlayer(0).camMods.Add(_cameraModifier);
		});
		_earthquakeTask = new TaskManager.Task(EarthquakeRoutine);
		MonoBehaviourCollector<NPC, CharacterCollectorItem>.ItemAdded += OnCharactersChanged;
		MonoBehaviourCollector<NPC, CharacterCollectorItem>.ItemRemoved += OnCharactersChanged;
		void OnCharactersChanged(CharacterCollectorItem _)
		{
			if (Deception)
			{
				Deception = false;
				Deception = true;
			}
		}
	}

	private IEnumerator EarthquakeRoutine()
	{
		while (Earthquake)
		{
			float num = EarthquakeStrength / 10f;
			_cameraModifier.posOffset = new Vector3(Random.Range(0f - num, num), Random.Range(0f - num, num), Random.Range(0f - num, num));
			yield return null;
		}
	}
}
