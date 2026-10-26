using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Patches;
using ModMenu.BaldiPlusEngineGame.Settings;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using ModMenu.Settings;
using PlusModMenu.Api;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using PlusModMenu.Api.Settings;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables.OtherModifiers;

[DataContract]
internal class WorldModifiers : IModifiers
{
	private static readonly FieldInfo _mapArrows = AccessTools.Field(typeof(Map), "arrows");

	private static readonly FieldInfo _grades = AccessTools.Field(typeof(CoreGameManager), "grades");

	private readonly HashSet<Entity> _arrowEntities = new HashSet<Entity>();

	private bool _npcMapArrows;

	[DataMember]
	[Label<Strings>("NPC_MAP_ARROWS")]
	[Help<HelpText>("NPC_MAP_ARROWS_TITLE", "NPC_MAP_ARROWS_TEXT")]
	[Group<SideMenuGroupOtherModifiersWorldMap>]
	public virtual bool NpcMapArrows
	{
		get
		{
			return _npcMapArrows;
		}
		set
		{
			_npcMapArrows = value;
			SyncArrows();
		}
	}

	private bool _freeStore;

	[DataMember]
	[Label<Strings>("FREE_STORE")]
	[Help<HelpText>("FREE_STORE_TITLE", "FREE_STORE_TEXT")]
	[Group<SideMenuGroupOtherModifiersWorldStore>]
	public virtual bool FreeStore
	{
		get
		{
			return _freeStore;
		}
		set
		{
			_freeStore = value;
			FreeStorePatch.ApplyAll(value);
		}
	}

	[DataMember]
	[Label<Strings>("MINIGAME_AUTO_WIN")]
	[Help<HelpText>("MINIGAME_AUTO_WIN_TITLE", "MINIGAME_AUTO_WIN_TEXT")]
	[Group<SideMenuGroupOtherModifiersWorldFieldTrips>]
	public virtual bool MinigameAutoWin { get; set; }

	[DataMember]
	[Label<Strings>("FAST_ELEVATOR")]
	[Help<HelpText>("FAST_ELEVATOR_TITLE", "FAST_ELEVATOR_TEXT")]
	[Group<SideMenuGroupOtherModifiersWorldElevator>]
	public virtual bool FastElevator { get; set; }

	private static EnvironmentController? Ec => Singleton<BaseGameManager>.Instance != null ? Singleton<BaseGameManager>.Instance.Ec : null;

	private static CoreGameManager? Core => Singleton<CoreGameManager>.Instance;

	protected WorldModifiers()
	{
		Service.Get<IEventService>().SubscribeOnce<IModMenuInitializedEvent>((object? _) => GenerateUI());
		CoreGameManagerPatch.PlayersSpawned += (EnvironmentController ec) =>
		{
			_arrowEntities.Clear();
			ModMenuManager.Run(KeepArrows(ec));
		};
	}

	private IEnumerator KeepArrows(EnvironmentController ec)
	{
		while (ec != null)
		{
			SyncArrows();
			yield return new WaitForSecondsRealtime(0.5f);
		}
	}

	private static readonly Dictionary<string, Color> _arrowColors = new();

	private static Color ArrowColor(NPC npc)
	{
		var key = npc.Character == Character.Null ? $"{npc.GetType().Name}/{npc.name.Replace("(Clone)", "").Trim()}" : npc.Character.ToString();
		if (_arrowColors.TryGetValue(key, out var known))
		{
			return known;
		}
		(float, float, float)? avoid = null;
		if (npc.Character != Character.Baldi && _arrowColors.TryGetValue(nameof(Character.Baldi), out var baldi))
		{
			avoid = (baldi.r, baldi.g, baldi.b);
		}
		var sprite = npc.spriteRenderer.Select(r => r != null ? r.sprite : null).FirstOrDefault(x => x != null)
			?? npc.GetComponentsInChildren<SpriteRenderer>(true).Select(r => r.sprite).FirstOrDefault(x => x != null);
		var colour = Color.white;
		if (sprite != null)
		{
			try
			{
				var (r, g, b) = SpriteColors.Dominant(FortyTwoPatch.Pixels(sprite).Select(c => (c.r, c.g, c.b, c.a)).ToList(), avoid);
				colour = new Color(r, g, b);
			}
			catch (Exception e)
			{
				Debug.LogWarning($"[ModMenu] no map colour for {npc.Character}: {e.Message}");
			}
		}
		_arrowColors[key] = colour;
		return colour;
	}

	private void SyncArrows()
	{
		var ec = Ec;
		var map = ec != null ? ec.map : null;
		if (map == null)
		{
			return;
		}
		var arrows = (List<MapIcon>)_mapArrows.GetValue(map);
		if (!_npcMapArrows)
		{
			for (var i = map.arrowTargets.Count - 1; i >= 0; i--)
			{
				if (map.arrowTargets[i] != null && _arrowEntities.Contains(map.arrowTargets[i]))
				{
					UnityEngine.Object.Destroy(arrows[i].gameObject);
					arrows.RemoveAt(i);
					map.arrowTargets.RemoveAt(i);
				}
			}
			_arrowEntities.Clear();
			return;
		}
		foreach (var npc in ec!.Npcs.OrderBy(n => n != null && n.Character == Character.Baldi ? 0 : 1))
		{
			var entity = npc != null ? npc.Entity : null;
			if (entity != null && !_arrowEntities.Contains(entity))
			{
				map.AddArrow(entity, ArrowColor(npc!));
				_arrowEntities.Add(entity);
			}
		}
	}

	private static void GivePoints(int points)
	{
		if (points == 0)
		{
			return;
		}
		if (points > 0)
		{
			var ytp = Resources.FindObjectsOfTypeAll<ItemObject>().FirstOrDefault(item => item.item is ITM_YTPs && item.audPickupOverride != null);
			if (ytp != null)
			{
				Core!.audMan.PlaySingle(ytp.audPickupOverride);
			}
		}
		Core!.AddPoints(points, 0, playAnimation: points > 0, includeInLevelTotal: false, multiply: false);
	}

	private void GenerateUI()
	{
		var registry = SettingsRegistryService.Registry;
		var notInGame = UIText.Resource<Strings>("NOT_AVAILABLE_NOW");
		var inGame = new List<ISettingHandle>();

		var survival = registry.GetGroup(typeof(PlayerModifiersSurvivalGroup));
		inGame.Add(survival.AddNumber(UIText.Resource<Strings>("YTP"), () => Core != null ? Core.GetPoints(0) : 0, x =>
		{
			if (Core != null)
			{
				GivePoints((int)x - Core.GetPoints(0));
			}
		}, new NumberOptions { Integer = true }, UIText.Resource<HelpText>("YTP_TEXT")));
		inGame.Add(survival.AddNumber(UIText.Resource<Strings>("LIVES"), () => Core != null ? Core.Lives : 0, x =>
		{
			Core?.SetLives((int)x, lifeModeOverride: true);
		}, new NumberOptions { Integer = true, Min = 0.0 }, UIText.Resource<HelpText>("LIVES_TEXT")));
		var grades = (string[])_grades.GetValue(null);
		inGame.Add(survival.AddChoice(UIText.Resource<Strings>("GRADE"), () => grades.Select(UIText.Literal).ToList(), () => Core != null ? Core.GradeVal : 0, x =>
		{
			if (Core != null)
			{
				Core.GradeVal = x;
			}
		}, UIText.Resource<HelpText>("GRADE_TEXT")));

		var map = registry.GetGroup(typeof(SideMenuGroupOtherModifiersWorldMap));
		inGame.Add(map.AddButton(UIText.Resource<Strings>("REVEAL_MAP"), () => Ec?.map?.CompleteMap(), UIText.Resource<HelpText>("REVEAL_MAP_TEXT")));

		var doors = registry.GetGroup(typeof(SideMenuGroupOtherModifiersWorldDoors));
		inGame.Add(doors.AddButton(UIText.Resource<Strings>("OPEN_ALL_DOORS"), () => ForEachDoor(door =>
		{
			door.Unlock();
			door.Open(cancelTimer: true, makeNoise: false);
		})));
		inGame.Add(doors.AddButton(UIText.Resource<Strings>("LOCK_ALL_DOORS"), () => ForEachDoor(door => door.Lock(cancelTimer: true))));
		inGame.Add(doors.AddButton(UIText.Resource<Strings>("UNLOCK_ALL_DOORS"), () => ForEachDoor(door => door.Unlock())));
		inGame.Add(doors.AddButton(UIText.Resource<Strings>("BREAK_ALL_WINDOWS"), () =>
		{
			foreach (var window in UnityEngine.Object.FindObjectsOfType<Window>())
			{
				window.Break(makeNoise: false);
			}
		}));
		inGame.Add(doors.AddButton(UIText.Resource<Strings>("OPEN_LOCKED_ROOMS"), () =>
		{
			foreach (var room in UnityEngine.Object.FindObjectsOfType<LockedRoomFunction>())
			{
				room.RemoveLocks();
			}
		}, UIText.Resource<HelpText>("OPEN_LOCKED_ROOMS_TEXT")));

		var floor = registry.GetGroup(typeof(SideMenuGroupOtherModifiersWorldFloor));
		inGame.Add(floor.AddButton(UIText.Resource<Strings>("OPEN_ALL_EXITS"), () =>
		{
			var elevators = Ec?.ElevatorManager;
			if (elevators != null)
			{
				elevators.SetTotalOutOfOrderElevators(0);
				elevators.SetAllElevators(ElevatorState.OpenForExit);
			}
		}, UIText.Resource<HelpText>("OPEN_ALL_EXITS_TEXT")));
		inGame.Add(floor.AddButton(UIText.Resource<Strings>("SOLVE_ALL_ACTIVITIES"), () => ActivitySolver.SolveAll(), UIText.Resource<HelpText>("SOLVE_ALL_ACTIVITIES_TEXT")));
		inGame.Add(floor.AddButton(UIText.Resource<Strings>("FINISH_FLOOR"), () =>
		{
			if (CanFinishFloor())
			{
				Singleton<BaseGameManager>.Instance.FinishLevel();
			}
		}, UIText.Resource<HelpText>("FINISH_FLOOR_TEXT")));

		foreach (var row in inGame)
		{
			row.SetAvailable(available: false, notInGame);
		}
		Service.Get<IEventService>().Subscribe<ISceneChangedEvent>((object? x) =>
		{
			foreach (var row in inGame)
			{
				row.SetAvailable(ModMenuManager.IsAnyGameScene(x), notInGame);
				row.Refresh();
			}
		});
	}

	internal static bool CanFinishFloor()
	{
		var manager = Singleton<BaseGameManager>.Instance;
		return manager != null && (manager is not MainGameManager || Core?.sceneObject?.nextLevel != null);
	}

	private static void ForEachDoor(Action<Door> action)
	{
		if (Ec == null)
		{
			return;
		}
		foreach (var door in Ec.standardDoors.ToList())
		{
			if (door != null)
			{
				action(door);
			}
		}
	}
}
