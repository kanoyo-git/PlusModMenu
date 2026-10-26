using System.Collections.Generic;
using System.Runtime.Serialization;
using ClassicYetAgain.Interfaces;
using ClassicYetAgain.Variables.Menus;
using PlusModMenu.Core.Common.Extensions;
using ModMenu.BaldiPlusEngineGame.Settings;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Characters.Interfaces;
using PlusModMenu.Api.Collectors;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using PlusModMenu.Api.Settings;

namespace ClassicYetAgain.Variables.CharacterModifiers;

[DataContract]
public class PrincipalModifiers : ICharacterGetsSpawnedInSpoopMode, ICharacterModifiers, IModifiers, ILethalTouch, IModifiersForCharacter<Principal>
{
	public class DetentionTime
	{
		public float Time { get; set; }
	}

	private readonly InstanceCollector<DetentionTime> _customDetentionTimes = new InstanceCollector<DetentionTime>();

	[DataMember]
	[Label<Strings>("LETHAL_TOUCH")]
	[Help<HelpText>("LETHAL_TOUCH_TITLE", "LETHAL_TOUCH_TEXT")]
	[Group<CharacterModifiersPrincipalGeneralGroup>]
	public virtual bool LethalTouch { get; set; }

	[DataMember]
	[Label<Strings>("CUSTOM_DETENTION_TIMES")]
	[Help<HelpText>("CUSTOM_DETENTION_TIMES_TITLE", "CUSTOM_DETENTION_TIMES_TEXT")]
	[Group<SideMenuGroupPrincipalModifiersGeneralDetentionTimes>]
	public virtual bool CustomDetentionTimesEnabled { get; set; }

	public IReadOnlyList<DetentionTime> CustomDetentionTimes => _customDetentionTimes.Collection;

	[DataMember]
	[Label<Strings>("ACTIVATE_IN_SPOOP_MODE")]
	[Help<HelpText>("ACTIVATE_IN_SPOOP_MODE_TITLE", "ACTIVATE_IN_SPOOP_MODE_TEXT")]
	[Group<CharacterModifiersPrincipalGeneralGroup>]
	public virtual bool ActivateInSpoopMode { get; set; } = true;

	public PrincipalModifiers()
	{
		Service.Get<IEventService>().SubscribeOnce<IModMenuInitializedEvent>((object? _) =>
		{
			GenerateDetentionTimeManagerUI();
		});
	}

	[Label<Strings>("ADD_DETENTION_TIME")]
	[Group<SideMenuGroupPrincipalModifiersGeneralDetentionTimes>]
	public virtual void AddDetentionTime()
	{
		_customDetentionTimes.Add(new DetentionTime());
	}

	private void GenerateDetentionTimeManagerUI()
	{
		ISettingsGroup group = SettingsRegistryService.Registry.GetGroup(typeof(SideMenuGroupPrincipalModifiersGeneralDetentionTimes));
		List<ISettingHandle> rows = new List<ISettingHandle>();
		_customDetentionTimes.ItemAdded += (DetentionTime _) =>
		{
			RefreshRows();
		};
		_customDetentionTimes.ItemRemoved += (DetentionTime _) =>
		{
			RefreshRows();
		};
		RefreshRows();
		void RefreshRows()
		{
			foreach (ISettingHandle row in rows)
			{
				row.Remove();
			}
			rows.Clear();
			for (int i = 0; i < _customDetentionTimes.Collection.Count; i++)
			{
				DetentionTime detentionTime = _customDetentionTimes.Collection[i];
				rows.Add(group.AddNumber(UIText.Resource<Strings>("DETENTION_TIME__INDEX", i), () => detentionTime.Time, x =>
				{
					detentionTime.Time = (float)x;
				}, default));
				rows.Add(group.AddButton(UIText.Resource<Strings>("REMOVE_DETENTION_TIME__INDEX", i), () =>
				{
					_customDetentionTimes.Remove(detentionTime);
				}));
			}
		}
	}
}
