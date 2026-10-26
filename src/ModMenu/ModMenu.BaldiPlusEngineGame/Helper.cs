using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Settings;
using ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Characters;
using PlusModMenu.Api.Collectors;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces.Services;
using PlusModMenu.Api.Settings;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame;

internal class Helper
{
	private static readonly MethodInfo _setGuilt = AccessTools.Method(typeof(NPC), "SetGuilt");

	public static Dictionary<Type, string> CharacterNames { get; } = new Dictionary<Type, string>
	{
		{
			typeof(Baldi),
			Strings.BALDI
		},
		{
			typeof(Beans),
			Strings.BEANS
		},
		{
			typeof(Bully),
			Strings.ITS_A_BULLY
		},
		{
			typeof(ChalkFace),
			Strings.CHALKLES
		},
		{
			typeof(Cumulo),
			Strings.CLOUDY_COPTER
		},
		{
			typeof(ArtsAndCrafters),
			Strings.ARTS_AND_CRAFTERS
		},
		{
			typeof(FirstPrize),
			Strings.FIRST_PRIZE
		},
		{
			typeof(GottaSweep),
			Strings.GOTTA_SWEEP
		},
		{
			typeof(NoLateTeacher),
			Strings.MRS_POMP
		},
		{
			typeof(Playtime),
			Strings.PLAYTIME
		},
		{
			typeof(Principal),
			Strings.PRINCIPAL_OF_THE_THING
		},
		{
			typeof(LookAtGuy),
			Strings.THE_TEST
		},
		{
			typeof(DrReflex),
			Strings.DR_REFLEX
		}
	};

	public static Dictionary<Type, Vector3> DeathcamOffset { get; } = new Dictionary<Type, Vector3>
	{
		{
			typeof(Baldi),
			Vector3.zero
		},
		{
			typeof(Beans),
			new Vector3(0f, -1f, 0f)
		},
		{
			typeof(Bully),
			new Vector3(0f, 2.5f, 0f)
		},
		{
			typeof(ChalkFace),
			new Vector3(0f, -1f, 0f)
		},
		{
			typeof(Cumulo),
			new Vector3(0f, -2f, -4f)
		},
		{
			typeof(ArtsAndCrafters),
			new Vector3(0f, 3f, -4f)
		},
		{
			typeof(FirstPrize),
			Vector3.zero
		},
		{
			typeof(GottaSweep),
			Vector3.zero
		},
		{
			typeof(NoLateTeacher),
			Vector3.zero
		},
		{
			typeof(Playtime),
			new Vector3(0f, -2f, 0f)
		},
		{
			typeof(Principal),
			new Vector3(0f, 1.2f, 0f)
		},
		{
			typeof(LookAtGuy),
			Vector3.zero
		},
		{
			typeof(DrReflex),
			Vector3.zero
		},
	};

	public static string GetCharacterName(Type characterType)
	{
		if (!CharacterNames.TryGetValue(characterType, out string value))
		{
			return characterType.Name;
		}
		return value;
	}

	public static void CreateCharacterGuiltMenu(IReadOnlyList<CharacterCollectorItem> collection, Type characterMenu)
	{
		Service.Get<IEventService>().SubscribeOnce<IModMenuInitializedEvent>((object? _) =>
		{
			ISettingsGroup group = SettingsRegistryService.Registry.GetPage(characterMenu).AddGroup(UIText.Resource<Strings>("GUILT"));
			int selectedRule = 0;
			float time = 5f;
			PrincipalModifiers.RuleChoices.Add(group.AddChoice(UIText.Resource<Strings>("RULE_TO_BREAK"), () => PrincipalModifiers.RuleIds.ConvertAll(x => UIText.Literal(x)), () => selectedRule, x =>
			{
				selectedRule = x;
			}));
			group.AddNumber(UIText.Resource<Strings>("GUILT_TIME"), () => time, x =>
			{
				time = (float)x;
			}, new NumberOptions
			{
				Min = 0.0
			}, UIText.Resource<HelpText>("GUILT_TIME_TEXT"));
			group.AddButton(UIText.Resource<Strings>("BREAK_RULE"), () =>
			{
				List<string> rules = PrincipalModifiers.RuleIds;
				if (rules.Count == 0)
				{
					return;
				}
				string rule = rules[Math.Min(selectedRule, rules.Count - 1)];
				foreach (CharacterCollectorItem item in collection)
				{
					_setGuilt.Invoke(item.Item, new object[2] { time, rule });
				}
			}, UIText.Resource<HelpText>("BREAK_RULE_TEXT"));
		});
	}

	public static ISettingsGroup CreateCharacterInstanceMenu<TNPC>(Type characterMenu, Action<int>? spawn = null) where TNPC : NPC
	{
		ISettingsGroup group = SettingsRegistryService.Registry.GetPage(characterMenu).AddGroup(UIText.Resource<Strings>("INSTANCE_EDITOR"));
		List<CharacterCollectorItem> instances = new List<CharacterCollectorItem>();
		int selected = 0;
		int newInstances = 1;
		UIText noInstances = UIText.Resource<Strings>("NOT_AVAILABLE_NOW");
		ISettingHandle selectedInstance = group.AddChoice(UIText.Resource<Strings>("SELECTED_INSTANCE"), () => instances.Count == 0 ? new List<UIText> { UIText.Resource<Strings>("NONE") } : instances.ConvertAll(x => UIText.Literal("#" + (instances.IndexOf(x) + 1))), () => Math.Min(selected, Math.Max(instances.Count - 1, 0)), x =>
		{
			selected = x;
		});
		selectedInstance.SetAvailable(available: false, noInstances);
		MonoBehaviourCollector<TNPC, CharacterCollectorItem>.ItemAdded += (CharacterCollectorItem x) =>
		{
			instances.Add(x);
			selectedInstance.SetAvailable(available: true);
			selectedInstance.Refresh();
		};
		MonoBehaviourCollector<TNPC, CharacterCollectorItem>.ItemRemoved += (CharacterCollectorItem x) =>
		{
			instances.Remove(x);
			selectedInstance.SetAvailable(instances.Count > 0, noInstances);
			selectedInstance.Refresh();
		};
		group.AddNumber(UIText.Resource<Strings>("NEW_INSTANCES"), () => newInstances, x =>
		{
			newInstances = (int)x;
		}, new NumberOptions
		{
			Integer = true,
			Min = 1.0
		}, UIText.Resource<HelpText>("SPAWN_TEXT"));
		group.AddButton(UIText.Resource<Strings>("SPAWN"), () =>
		{
			if (spawn != null)
			{
				spawn(newInstances);
			}
			else
			{
				AbstractCharacterModifiers<TNPC>.Spawn(newInstances);
			}
		}, UIText.Resource<HelpText>("SPAWN_TEXT"));
		group.AddButton(UIText.Resource<Strings>("DESPAWN_ALL_INSTANCES"), AbstractCharacterModifiers<TNPC>.Despawn, UIText.Resource<HelpText>("DESPAWN_TEXT"));
		group.AddButton(UIText.Resource<Strings>("DESPAWN_SELECTED_INSTANCE"), () =>
		{
			Selected()?.Item.Despawn();
		}, UIText.Resource<HelpText>("DESPAWN_TEXT"));
		group.AddButton(UIText.Resource<Strings>("TELEPORT_PLAYER_TO_SELECTED_INSTANCE"), () =>
		{
			CharacterCollectorItem? item = Selected();
			if (item != null)
			{
				Singleton<CoreGameManager>.Instance.GetPlayer(0).Teleport(item.Item.transform.position);
			}
		}, UIText.Resource<HelpText>("TELEPORT_PLAYER_TO_CHARACTER_TEXT"));
		group.AddButton(UIText.Resource<Strings>("TELEPORT_ALL_INSTANCES_TO_PLAYER"), () =>
		{
			foreach (CharacterCollectorItem item in MonoBehaviourCollector<TNPC, CharacterCollectorItem>.Collection)
			{
				item.Item.Entity.Teleport(Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position);
			}
		}, UIText.Resource<HelpText>("TELEPORT_CHARACTER_TO_PLAYER_TEXT"));
		group.AddButton(UIText.Resource<Strings>("TELEPORT_SELECTED_INSTANCE_TO_PLAYER"), () =>
		{
			Selected()?.Item.Entity.Teleport(Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position);
		}, UIText.Resource<HelpText>("TELEPORT_CHARACTER_TO_PLAYER_TEXT"));
		return group;
		CharacterCollectorItem? Selected()
		{
			return instances.Count == 0 ? null : instances[Math.Min(selected, instances.Count - 1)];
		}
	}
}
