using ModMenu.BaldiPlusEngineGame.Patches;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using Newtonsoft.Json;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables.OtherModifiers;

[JsonObject(MemberSerialization.OptIn)]
internal class PlayerDataModifiers : IModifiers
{
	[Label<Strings>("AUTHENTIC_MODE")]
	[Help<HelpText>("AUTHENTIC_MODE_TITLE", "AUTHENTIC_MODE_TEXT")]
	[Group<SideMenuGroupOtherModifiersPlayerDataSecrets>]
	public virtual bool AuthenticMode
	{
		get
		{
			return Singleton<PlayerFileManager>.Instance.authenticMode;
		}
		set
		{
			Singleton<PlayerFileManager>.Instance.authenticMode = value;
			Singleton<PlayerFileManager>.Instance.Save();
		}
	}

	[Label<Strings>("DEBUG_MODE")]
	[Help<HelpText>("DEBUG_MODE_TITLE", "DEBUG_MODE_TEXT")]
	[Group<SideMenuGroupOtherModifiersPlayerDataSecrets>]
	public virtual bool DebugMode
	{
		get
		{
			return DebugOverlay.Shown;
		}
		set
		{
			DebugOverlay.Shown = value;
		}
	}
}
