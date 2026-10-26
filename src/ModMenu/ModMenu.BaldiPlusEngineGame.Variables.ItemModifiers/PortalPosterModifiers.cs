using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces;

namespace ModMenu.BaldiPlusEngineGame.Variables.ItemModifiers;

[DataContract]
internal class PortalPosterModifiers : IModifiers
{
	[DataMember]
	[Label<Strings>("ALLOW_ON_ANY_WALL")]
	[Help<HelpText>("ALLOW_ON_ANY_WALL_TITLE", "ALLOW_ON_ANY_WALL_TEXT")]
	[Group<ItemModifiersPortalPosterGeneralGroup>]
	public virtual bool AllowOnAnyWall { get; set; }
}
