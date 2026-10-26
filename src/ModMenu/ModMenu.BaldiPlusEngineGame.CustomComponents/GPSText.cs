using ModMenu.BaldiPlusEngineGame.Collectors;
using ModMenu.BaldiPlusEngineGame.Variables.OtherModifiers;
using ModMenu.Common.CustomComponents;
using PlusModMenu.Api;
using PlusModMenu.Api.Collectors;
using PlusModMenu.Api.Interfaces.Services;

namespace ModMenu.BaldiPlusEngineGame.CustomComponents;

public class GPSText : ModMenu.Common.CustomComponents.GPSText
{
	public new bool Active
	{
		set
		{
			gameObject.SetActive(value);
		}
	}

	public new void Awake()
	{
		MonoBehaviourCollector<GPSText, GPSTextCollectorItem>.Add(this);
		base.Awake();
	}

	private void OnEnable()
	{
		Active = Service.Get<IModMenuService>().Get<OtherModifiers>().EntityGPS;
	}
}
