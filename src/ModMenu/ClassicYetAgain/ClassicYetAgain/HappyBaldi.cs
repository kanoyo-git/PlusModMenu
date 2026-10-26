using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClassicYetAgain.Patches;
using ClassicYetAgain.Variables.CharacterModifiers;
using PlusModMenu.Core.Common;
using PlusModMenu.Core.Unity;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ClassicYetAgain;

public class HappyBaldi : NPC
{
	private const int BASE_SPEED = 10;

	private static readonly FieldInfo _character;

	private static readonly FieldInfo _npc;

	private static readonly List<Action<HappyBaldi>> _pendingTemplateRequests = new List<Action<HappyBaldi>>();

	private static GameObject? _neverRoot;

	private static HappyBaldi? _template;

	private static bool _creatingTemplate;

	private readonly MovementModifier _movementModifier = new MovementModifier(Vector3.zero, 1f);

	private NavigationState_WanderRandom _wanderState;

	private NavigationState_TargetPlayer _targetState;

	private static GameObject NeverRoot
	{
		get
		{
			if (_neverRoot == null)
			{
				_neverRoot = new GameObject("Never Root");
				_neverRoot.SetActive(value: false);
				UnityEngine.Object.DontDestroyOnLoad(_neverRoot);
			}
			return _neverRoot;
		}
	}

	static HappyBaldi()
	{
		_character = AccessTools.Field(typeof(NPC), "character");
		_npc = AccessTools.Field(typeof(Looker), "npc");
		ModMenu.BaldiPlusEngineGame.Helper.CharacterNames.Add(typeof(HappyBaldi), Strings.HAPPY_BALDI);
	}

	public static void WithTemplate(Action<HappyBaldi> then)
	{
		if (_template != null)
		{
			then(_template);
			return;
		}
		_pendingTemplateRequests.Add(then);
		if (!_creatingTemplate)
		{
			_creatingTemplate = true;
			UnityThread.ExecuteCoroutine(CreateTemplate());
		}
	}

	private static IEnumerator CreateTemplate()
	{
		Baldi baldiPrefab = (from x in Resources.FindObjectsOfTypeAll<Baldi>()
			where x.GetType() == typeof(Baldi) && !x.gameObject.scene.IsValid()
			orderby x.name != "Baldi"
			select x).FirstOrDefault();
		SpriteRenderer happySprite = (from x in Resources.FindObjectsOfTypeAll<global::HappyBaldi>()
			where !x.gameObject.scene.IsValid()
			select x.GetComponentInChildren<SpriteRenderer>(includeInactive: true)).FirstOrDefault((SpriteRenderer x) => x != null);
		if (baldiPrefab == null || happySprite == null)
		{
			Debugger.Warning("HappyBaldi: the Baldi or Happy Baldi prefab couldn't be found, Happy Baldi can't be spawned.");
			_pendingTemplateRequests.Clear();
			_creatingTemplate = false;
			yield break;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(baldiPrefab.gameObject, NeverRoot.transform);
		gameObject.name = "HappyBaldi";
		for (int i = 0; i < 3; i++)
		{
			yield return null;
		}
		Baldi baldi = gameObject.GetComponent<Baldi>();
		Navigator navigator = baldi.Navigator;
		Looker looker = baldi.looker;
		Collider[] baseTrigger = baldi.baseTrigger;
		GameObject spriteBase = baldi.spriteBase;
		SpriteRenderer spriteRenderer = baldi.spriteRenderer.FirstOrDefault((SpriteRenderer x) => x != null) ?? gameObject.GetComponentInChildren<SpriteRenderer>(includeInactive: true);
		List<RoomCategory> spawnableRooms = new List<RoomCategory>(baldi.spawnableRooms);
		foreach (VolumeAnimator item in gameObject.GetComponentsInChildren<VolumeAnimator>(includeInactive: true))
		{
			UnityEngine.Object.DestroyImmediate(item);
		}
		foreach (Animator item2 in gameObject.GetComponentsInChildren<Animator>(includeInactive: true))
		{
			UnityEngine.Object.DestroyImmediate(item2);
		}
		UnityEngine.Object.DestroyImmediate(baldi);
		HappyBaldi happyBaldi = gameObject.AddComponent<HappyBaldi>();
		happyBaldi.navigator = navigator;
		happyBaldi.looker = looker;
		happyBaldi.baseTrigger = baseTrigger;
		happyBaldi.spriteBase = spriteBase;
		happyBaldi.spriteRenderer = new SpriteRenderer[1] { spriteRenderer };
		happyBaldi.spawnableRooms = spawnableRooms;
		_character.SetValue(happyBaldi, Character.Null);
		navigator.npc = happyBaldi;
		_npc.SetValue(looker, happyBaldi);
		spriteRenderer.sprite = happySprite.sprite;
		spriteRenderer.transform.localPosition = happySprite.transform.localPosition;
		spriteRenderer.transform.localScale = happySprite.transform.localScale;
		_template = happyBaldi;
		_creatingTemplate = false;
		Action<HappyBaldi>[] array = _pendingTemplateRequests.ToArray();
		_pendingTemplateRequests.Clear();
		foreach (Action<HappyBaldi> action in array)
		{
			action(happyBaldi);
		}
	}

	public override void Initialize()
	{
		base.Initialize();
		navigator.maxSpeed = BASE_SPEED;
		navigator.SetSpeed(BASE_SPEED);
		navigator.Am.moveMods.Add(_movementModifier);
		_wanderState = new NavigationState_WanderRandom(this, 0);
		_targetState = new NavigationState_TargetPlayer(this, 0, base.transform.position);
		behaviorStateMachine.ChangeState(new NpcState(this));
		behaviorStateMachine.ChangeNavigationState(_wanderState);
	}

	protected override void VirtualUpdate()
	{
		HappyBaldiModifiers happyBaldiModifiers = Service.Get<IModMenuService>().Get<HappyBaldiModifiers>();
		if (!happyBaldiModifiers.SlenderBaldi)
		{
			_movementModifier.movementMultiplier = 0f;
			return;
		}
		if (happyBaldiModifiers.IsBlind)
		{
			behaviorStateMachine.ChangeNavigationState(_wanderState);
		}
		else
		{
			PlayerManager player = Singleton<CoreGameManager>.Instance.GetPlayer(0);
			if (player != null)
			{
				_targetState.UpdatePosition(player.transform.position);
				behaviorStateMachine.ChangeNavigationState(_targetState);
			}
		}
		_movementModifier.movementMultiplier = ((!spriteRenderer[0].isVisible || happyBaldiModifiers.MovesInSight || happyBaldiModifiers.IsBlind) ? 1 : 0);
	}

	protected override void VirtualOnTriggerEnter(Entity otherEntity, Collider other)
	{
		if (other.CompareTag("Player"))
		{
			HappyBaldiModifiers happyBaldiModifiers = Service.Get<IModMenuService>().Get<HappyBaldiModifiers>();
			if (happyBaldiModifiers.SlenderBaldi && !happyBaldiModifiers.IgnoresPlayerTrigger)
			{
				CoreGameManagerPatch.Helper.EndGame(other.transform, this);
			}
		}
	}
}
