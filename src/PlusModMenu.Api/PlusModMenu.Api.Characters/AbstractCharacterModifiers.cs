using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using PlusModMenu.Core.Unity;
using HarmonyLib;
using PlusModMenu.Api.Characters.Interfaces;
using PlusModMenu.Api.Collectors;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using PlusModMenu.Api.Internal;
using UnityEngine;

namespace PlusModMenu.Api.Characters;

[DataContract]
public abstract class AbstractCharacterModifiers<TNPC> : IBasicCharacter, ICharacterCanBeToggled, ICharacterModifiers, IModifiers, IModifiersForCharacter<TNPC> where TNPC : NPC
{
	public Type Type { get; } = typeof(TNPC);

	public bool Exists => MonoBehaviourCollector<TNPC, CharacterCollectorItem>.Collection.Count > 0;

	public IReadOnlyList<CharacterCollectorItem> Collection => MonoBehaviourCollector<TNPC, CharacterCollectorItem>.Collection;

	public virtual bool Enabled
	{
		get
		{
			return MonoBehaviourCollector<TNPC, CharacterCollectorItem>.Collection.Any((CharacterCollectorItem x) => IsEntityActive(x.Item));
		}
		set
		{
			SubtitleController[] source = Resources.FindObjectsOfTypeAll<SubtitleController>();
			foreach (CharacterCollectorItem c in Collection)
			{
				foreach (SubtitleController item in source.Where((SubtitleController x) => x.soundTran == c.transform))
				{
					item.gameObject.SetActive(value);
				}
				Entity entity = c.Item.Entity;
				if (entity != null)
				{
					entity.SetActive(value);
				}
				else
				{
					c.gameObject.SetActive(value);
				}
			}
		}
	}

	private static readonly FieldInfo _entityActive = AccessTools.Field(typeof(Entity), "active");

	private static bool IsEntityActive(NPC npc)
	{
		Entity entity = npc.Entity;
		if (entity == null)
		{
			return npc.gameObject.activeSelf;
		}
		return (bool)_entityActive.GetValue(entity);
	}

	protected AbstractCharacterModifiers()
	{
	}

	public static void Spawn(int count, TNPC? customInstance = null)
	{
		if (Singleton<CoreGameManager>.Instance == null)
		{
			return;
		}
		PlayerManager player = Singleton<CoreGameManager>.Instance.GetPlayer(0);
		if (!(player == null))
		{
			TNPC val = ((!(customInstance != null)) ? (Internals.Characters.FirstOrDefault((NPC x) => x.GetType() == typeof(TNPC)) as TNPC) : customInstance);
			if (val == null)
			{
				throw new NullReferenceException($"An NPC of the requested Type ({typeof(TNPC)}) couldn't be found.");
			}
			for (int num = 0; num < count; num++)
			{
				Singleton<BaseGameManager>.Instance.Ec.SpawnNPC((NPC)val, IntVector2.GetGridPosition(player.transform.position));
				new TaskManager.SimpleTask(WaitForDistance(Singleton<BaseGameManager>.Instance.Ec.Npcs.Last())).Start();
			}
		}
		static IEnumerator WaitForDistance(NPC npc)
		{
			IEnumerable<Collider> colliders = (from x in npc.GetComponentsInChildren<Collider>()
				where x.enabled
				select x).ToArray();
			Looker looker = npc.GetComponentInChildren<Looker>();
			npc.enabled = false;
			foreach (Collider item5 in colliders)
			{
				item5.enabled = false;
			}
			looker.enabled = false;
			Transform npcTransform = npc.transform;
			Transform transform = Singleton<CoreGameManager>.Instance.GetPlayer(0).transform;
			(SpriteRenderer, Color)[] spritesAndColors = (from x in npc.GetComponentsInChildren<SpriteRenderer>()
				select (x: x, color: x.color)).ToArray();
			(SpriteRenderer, Color)[] array = spritesAndColors;
			for (int num2 = 0; num2 < array.Length; num2++)
			{
				(SpriteRenderer, Color) tuple = array[num2];
				SpriteRenderer item = tuple.Item1;
				Color item2 = tuple.Item2;
				Color color = item2;
				color.a = 0.5f;
				item.color = color;
			}
			yield return OtherUtils.WaitForDistance(npcTransform, transform, Internals.CharacterActivationDistance);
			if ((bool)npcTransform)
			{
				array = spritesAndColors;
				for (int num2 = 0; num2 < array.Length; num2++)
				{
					(SpriteRenderer, Color) tuple2 = array[num2];
					SpriteRenderer item3 = tuple2.Item1;
					Color item4 = tuple2.Item2;
					item3.color = item4;
				}
				npc.enabled = true;
				foreach (Collider item6 in colliders)
				{
					item6.enabled = true;
				}
				looker.enabled = true;
			}
		}
	}

	public static void Despawn()
	{
		foreach (CharacterCollectorItem item in MonoBehaviourCollector<TNPC, CharacterCollectorItem>.Collection)
		{
			item.Item.Despawn();
		}
	}
}
