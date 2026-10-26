using System;
using System.Collections.Generic;
using System.Linq;
using ModMenu.BaldiPlusEngineGame.Collectors;
using ModMenu.BaldiPlusEngineGame.Patches;
using ModMenu.BaldiPlusEngineGame.Settings;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api;
using PlusModMenu.Api.Characters;
using PlusModMenu.Api.Characters.Interfaces;
using PlusModMenu.Api.Collectors;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces.Services;
using PlusModMenu.Api.Settings;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame;

public class Teleporter
{
	public static void Init()
	{
		GenerateCharacterTeleporterUI();
		GenerateNotebookTeleporterUI();
		GenerateExitTeleporterUI();
		GenerateClassTeleporterUI();
	}

	private static void GenerateCharacterTeleporterUI()
	{
		List<ISettingHandle> buttons = new List<ISettingHandle>();
		ISettingsRegistry registry = SettingsRegistryService.Registry;
		IEnumerable<IBasicCharacter> enumerable = Service.Get<IModMenuService>().Modifiers.OfType<IBasicCharacter>();
		ISettingsGroup groupWidget = registry.GetGroup(typeof(TeleporterCharacterToPlayerGroup));
		ISettingsGroup groupWidget2 = registry.GetGroup(typeof(TeleporterPlayerToCharacterGroup));
		foreach (IBasicCharacter characterModifier in enumerable)
		{
			CreateButton(characterModifier, groupWidget, () =>
			{
				NPC nPC = characterModifier.Collection.FirstOrDefault((CharacterCollectorItem x) => x != null)?.Item;
				if (nPC != null)
				{
					Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position = nPC.transform.position;
				}
			});
			CreateButton(characterModifier, groupWidget2, () =>
			{
				foreach (CharacterCollectorItem item in characterModifier.Collection)
				{
					item.Item.transform.position = Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position;
				}
			});
		}
		Service.Get<IEventService>().Subscribe<ISceneChangedEvent>((object? x) =>
		{
			foreach (ISettingHandle item2 in buttons)
			{
				item2.Visible = ModMenuManager.IsAnyGameScene(x);
			}
		});
		void CreateButton(IBasicCharacter basicCharacter, ISettingsGroup group, Action click)
		{
			ISettingHandle button = group.AddButton(UIText.Literal(Helper.GetCharacterName(basicCharacter.Type)), click);
			button.Visible = false;
			buttons.Add(button);
		}
	}

	private static void GenerateNotebookTeleporterUI()
	{
		Dictionary<NotebookCollectorItem, List<ISettingHandle>> buttons = new Dictionary<NotebookCollectorItem, List<ISettingHandle>>();
		ISettingsRegistry registry = SettingsRegistryService.Registry;
		ISettingsGroup notebookToPlayerGroup = registry.GetGroup(typeof(TeleporterNotebookToPlayerGroup));
		ISettingsGroup playerToNotebookGroup = registry.GetGroup(typeof(TeleporterPlayerToNotebookGroup));
		MonoBehaviourCollector<Notebook, NotebookCollectorItem>.ItemAdded += (NotebookCollectorItem x) =>
		{
			int count = MonoBehaviourCollector<Notebook, NotebookCollectorItem>.Collection.Count;
			buttons.Add(x, new List<ISettingHandle>
			{
				CreateButton(count, notebookToPlayerGroup, () =>
				{
					ActivitySolver.Solve(x.Item.activity);
					x.Item.transform.position = Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position;
				}),
				CreateButton(count, playerToNotebookGroup, () =>
				{
					ActivitySolver.Solve(x.Item.activity);
					Singleton<CoreGameManager>.Instance.GetPlayer(0).Teleport(x.Item.transform.position);
				})
			});
		};
		MonoBehaviourCollector<Notebook, NotebookCollectorItem>.ItemRemoved += (NotebookCollectorItem x) =>
		{
			if (!buttons.TryGetValue(x, out List<ISettingHandle> handles))
			{
				return;
			}
			foreach (ISettingHandle item in handles)
			{
				item.Remove();
			}
			buttons.Remove(x);
		};
		static ISettingHandle CreateButton(int notebookIndex, ISettingsGroup group, Action click)
		{
			return group.AddButton(UIText.Resource<Strings>("NOTEBOOK__INDEX", notebookIndex), click, notebookIndex);
		}
	}

	private static void GenerateExitTeleporterUI()
	{
		Dictionary<ElevatorCollectorItem, List<ISettingHandle>> buttons = new Dictionary<ElevatorCollectorItem, List<ISettingHandle>>();
		ISettingsRegistry registry = SettingsRegistryService.Registry;
		ISettingsGroup playerToElevatorGroup = registry.GetGroup(typeof(TeleporterPlayerToElevatorGroup));
		MonoBehaviourCollector<Elevator, ElevatorCollectorItem>.ItemAdded += (ElevatorCollectorItem x) =>
		{
			int count = MonoBehaviourCollector<Elevator, ElevatorCollectorItem>.Collection.Count;
			buttons.Add(x, new List<ISettingHandle> { CreateButton(count, playerToElevatorGroup, () =>
			{
				Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position = x.Item.transform.position + new Vector3(0f, 5f, 0f);
			}) });
		};
		MonoBehaviourCollector<Elevator, ElevatorCollectorItem>.ItemRemoved += (ElevatorCollectorItem x) =>
		{
			if (!buttons.TryGetValue(x, out List<ISettingHandle> handles))
			{
				return;
			}
			foreach (ISettingHandle item in handles)
			{
				item.Remove();
			}
			buttons.Remove(x);
		};
		static ISettingHandle CreateButton(int elevatorIndex, ISettingsGroup group, Action click)
		{
			return group.AddButton(UIText.Resource<Strings>("EXIT__INDEX", elevatorIndex), click, elevatorIndex);
		}
	}

	private static void GenerateClassTeleporterUI()
	{
		var buttons = new List<ISettingHandle>();
		var group = SettingsRegistryService.Registry.GetGroup(typeof(TeleporterPlayerToClassGroup));
		CoreGameManagerPatch.PlayersSpawned += (EnvironmentController ec) =>
		{
			foreach (var handle in buttons)
			{
				handle.Remove();
			}
			buttons.Clear();
			var classes = ec.rooms.Where(room => room != null && room.category == RoomCategory.Class).ToList();
			for (var i = 0; i < classes.Count; i++)
			{
				var room = classes[i];
				var index = i + 1;
				buttons.Add(group.AddButton(UIText.Resource<Strings>("CLASS__INDEX", index), () => ToRoom(room), index));
			}
		};
	}

	internal static void ToRoom(RoomController room)
	{
		if (room == null)
		{
			return;
		}
		var cells = room.AllTilesNoGarbage(false, false);
		if (cells.Count == 0)
		{
			cells = room.cells;
		}
		var centre = cells.Aggregate(Vector3.zero, (sum, cell) => sum + cell.FloorWorldPosition) / cells.Count;
		var cell = cells.OrderBy(c => (c.FloorWorldPosition - centre).sqrMagnitude).First();
		Singleton<CoreGameManager>.Instance.GetPlayer(0).Teleport(cell.FloorWorldPosition + Vector3.up * 5f);
	}
}
