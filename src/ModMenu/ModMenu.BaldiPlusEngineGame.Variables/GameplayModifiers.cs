using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Patches;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces;

namespace ModMenu.BaldiPlusEngineGame.Variables;

[DataContract]
internal class GameplayModifiers : IModifiers
{
	private readonly TimeScaleModifier _generalTimescaleModifier = new TimeScaleModifier();

	internal TimeScaleModifier SuperHotTimeScaleModifier { get; } = new TimeScaleModifier();

	[DataMember]
	[Label<Strings>("ENVIRONMENT_TIMESCALE")]
	[Help<HelpText>("ENVIRONMENT_TIMESCALE_TITLE", "ENVIRONMENT_TIMESCALE_TEXT")]
	[Group<GameplayModifiersTimescaleGroup>]
	public virtual float EnvironmentTimeScale
	{
		get
		{
			return _generalTimescaleModifier.environmentTimeScale;
		}
		set
		{
			_generalTimescaleModifier.environmentTimeScale = value;
		}
	}

	[DataMember]
	[Label<Strings>("NPC_TIMESCALE")]
	[Help<HelpText>("NPC_TIMESCALE_TITLE", "NPC_TIMESCALE_TEXT")]
	[Group<GameplayModifiersTimescaleGroup>]
	public virtual float NPCTimeScale
	{
		get
		{
			return _generalTimescaleModifier.npcTimeScale;
		}
		set
		{
			_generalTimescaleModifier.npcTimeScale = value;
		}
	}

	[DataMember]
	[Label<Strings>("SUPER_HOT")]
	[Help<HelpText>("SUPER_HOT_TITLE", "SUPER_HOT_TEXT")]
	[Group<GameplayModifiersTimescaleGroup>]
	public virtual bool SuperHot { get; set; }

	protected GameplayModifiers()
	{
		CoreGameManagerPatch.PlayersSpawned += (EnvironmentController ec) =>
		{
			ec.AddTimeScale(_generalTimescaleModifier);
			ec.AddTimeScale(SuperHotTimeScaleModifier);
		};
	}
}
