using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Collectors;
using ModMenu.BaldiPlusEngineGame.CustomComponents;
using ModMenu.BaldiPlusEngineGame.Patches;
using ModMenu.BaldiPlusEngineGame.Settings;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using ModMenu.Common;
using ModMenu.Common.CustomComponents;
using PlusModMenu.Api;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Collectors;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using PlusModMenu.Api.Settings;

namespace ModMenu.BaldiPlusEngineGame.Variables.OtherModifiers;

[DataContract]
internal class OtherModifiers : IModifiers
{
	private bool _entityGps;

	private bool _showFps;

	private bool _fourtyTwo;

	private bool _disableHud;

	[DataMember]
	[Label<Strings>("ENTITY_GPS")]
	[Help<HelpText>("ENTITY_GPS_TITLE", "ENTITY_GPS_TEXT")]
	[Group<SideMenuGroupOtherModifiersGeneral>]
	public virtual bool EntityGPS
	{
		get
		{
			return _entityGps;
		}
		set
		{
			_entityGps = value;
			foreach (GPSTextCollectorItem item in MonoBehaviourCollector<ModMenu.BaldiPlusEngineGame.CustomComponents.GPSText, GPSTextCollectorItem>.Collection)
			{
				item.Item.Active = value;
			}
		}
	}

	[DataMember]
	[Label<Strings>("SHOW_ENTITY_DISTANCE")]
	[Help<HelpText>("SHOW_ENTITY_DISTANCE_TITLE", "SHOW_ENTITY_DISTANCE_TEXT")]
	[Group<SideMenuGroupOtherModifiersGeneral>]
	public virtual bool EntityDistance
	{
		get
		{
			return ModMenu.Common.CustomComponents.GPSText.ShowDistance;
		}
		set
		{
			ModMenu.Common.CustomComponents.GPSText.ShowDistance = value;
		}
	}

	[DataMember]
	[Label<Strings>("SHOW_FPS")]
	[Help<HelpText>("SHOW_FPS_TITLE", "SHOW_FPS_TEXT")]
	[Group<SideMenuGroupOtherModifiersGeneral>]
	public virtual bool ShowFps
	{
		get
		{
			return _showFps;
		}
		set
		{
			_showFps = value;
			FrameCounter.Instance.enabled = value;
		}
	}

	[DataMember]
	[Label<Strings>("FOURTY_TWO")]
	[Help<HelpText>("FOURTY_TWO_TITLE", "FOURTY_TWO_TEXT")]
	[Group<SideMenuGroupOtherModifiersGeneral>]
	public virtual bool FourtyTwo
	{
		get
		{
			return _fourtyTwo;
		}
		set
		{
			_fourtyTwo = value;
		}
	}

	[DataMember]
	[Label<Strings>("DISABLE_HUD")]
	[Help<HelpText>("DISABLE_HUD_TITLE", "DISABLE_HUD_TEXT")]
	[Group<SideMenuGroupOtherModifiersGeneral>]
	public virtual bool DisableHud
	{
		get
		{
			return _disableHud;
		}
		set
		{
			_disableHud = value;
			if (!(Singleton<CoreGameManager>.Instance == null))
			{
				Singleton<CoreGameManager>.Instance.GetHud(0).Hide(value);
			}
		}
	}

	protected OtherModifiers()
	{
		CoreGameManagerPatch.PlayersSpawned += (EnvironmentController _) =>
		{
			Singleton<CoreGameManager>.Instance.GetHud(0).Hide(DisableHud);
		};
		ISettingsGroup group = SettingsRegistryService.Registry.GetGroup(typeof(SideMenuGroupOtherModifiersGeneral));
		int notebooksToCollect = 1;
		group.AddNumber(UIText.Resource<Strings>("COLLECT_NOTEBOOKS"), () => notebooksToCollect, x =>
		{
			notebooksToCollect = (int)x;
		}, new NumberOptions
		{
			Integer = true,
			Min = 1.0
		}, UIText.Resource<HelpText>("COLLECT_NOTEBOOKS_TEXT"));
		group.AddButton(UIText.Resource<Strings>("COLLECT"), () =>
		{
			if (!(Singleton<BaseGameManager>.Instance == null))
			{
				Singleton<BaseGameManager>.Instance.CollectNotebooks(notebooksToCollect);
			}
		}, UIText.Resource<HelpText>("COLLECT_NOTEBOOKS_TEXT"));
	}
}
