using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace ClassicYetAgain;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Strings
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				resourceMan = ModMenu.BaldiPlusEngineGame.Settings.Localization.Manager("ClassicYetAgain.Strings", typeof(Strings).Assembly);
			}
			return resourceMan;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	internal static string ACTIVATE_IN_SPOOP_MODE => ResourceManager.GetString("ACTIVATE_IN_SPOOP_MODE", resourceCulture);

	internal static string ADD_DETENTION_TIME => ResourceManager.GetString("ADD_DETENTION_TIME", resourceCulture);

	internal static string ADD_NEW_CAMERA => ResourceManager.GetString("ADD_NEW_CAMERA", resourceCulture);

	internal static string AGGRESSIVE_PLAYTIME => ResourceManager.GetString("AGGRESSIVE_PLAYTIME", resourceCulture);

	internal static string BLIND => ResourceManager.GetString("BLIND", resourceCulture);

	internal static string CAMERA => ResourceManager.GetString("CAMERA", resourceCulture);

	internal static string CAMERA_MODIFIERS => ResourceManager.GetString("CAMERA_MODIFIERS", resourceCulture);

	internal static string CAMERAS => ResourceManager.GetString("CAMERAS", resourceCulture);

	internal static string CUSTOM_DETENTION_TIMES => ResourceManager.GetString("CUSTOM_DETENTION_TIMES", resourceCulture);

	internal static string CUSTOM_JEALOUSY => ResourceManager.GetString("CUSTOM_JEALOUSY", resourceCulture);

	internal static string DECEPTION => ResourceManager.GetString("DECEPTION", resourceCulture);

	internal static string DETENTION_TIME__INDEX => ResourceManager.GetString("DETENTION_TIME__INDEX", resourceCulture);

	internal static string EARTHQUAKE => ResourceManager.GetString("EARTHQUAKE", resourceCulture);

	internal static string EARTHQUAKE_STRENGTH => ResourceManager.GetString("EARTHQUAKE_STRENGTH", resourceCulture);

	internal static string ENABLE_PLAYER_CAMERA => ResourceManager.GetString("ENABLE_PLAYER_CAMERA", resourceCulture);

	internal static string ENABLED => ResourceManager.GetString("ENABLED", resourceCulture);

	internal static string ENFORCE_SELECTED_SKYBOX => ResourceManager.GetString("ENFORCE_SELECTED_SKYBOX", resourceCulture);

	internal static string FLIPAROO => ResourceManager.GetString("FLIPAROO", resourceCulture);

	internal static string GENERAL => ResourceManager.GetString("GENERAL", resourceCulture);

	internal static string HAPPY_BALDI => ResourceManager.GetString("HAPPY_BALDI", resourceCulture);

	internal static string IGNORES_PLAYER_TRIGGER => ResourceManager.GetString("IGNORES_PLAYER_TRIGGER", resourceCulture);

	internal static string INVISIBLE => ResourceManager.GetString("INVISIBLE", resourceCulture);

	internal static string JUMP_ANYWHERE => ResourceManager.GetString("JUMP_ANYWHERE", resourceCulture);

	internal static string LETHAL_TOUCH => ResourceManager.GetString("LETHAL_TOUCH", resourceCulture);

	internal static string LETHAL_TOUCH_ON_GUM_WAD => ResourceManager.GetString("LETHAL_TOUCH_ON_GUM_WAD", resourceCulture);

	internal static string MOVES_WHILE_BEING_LOOKED_AT => ResourceManager.GetString("MOVES_WHILE_BEING_LOOKED_AT", resourceCulture);

	internal static string OTHER => ResourceManager.GetString("OTHER", resourceCulture);

	internal static string OVERRIDE_JEALOUSY => ResourceManager.GetString("OVERRIDE_JEALOUSY", resourceCulture);

	internal static string PLAYTIME_DELIVERY_SERVICE => ResourceManager.GetString("PLAYTIME_DELIVERY_SERVICE", resourceCulture);

	internal static string RANDOMIZE_CHARACTER_POSITIONS => ResourceManager.GetString("RANDOMIZE_CHARACTER_POSITIONS", resourceCulture);

	internal static string RANDOMIZE_ITEM_POSITIONS => ResourceManager.GetString("RANDOMIZE_ITEM_POSITIONS", resourceCulture);

	internal static string REMOVE => ResourceManager.GetString("REMOVE", resourceCulture);

	internal static string SCALE => ResourceManager.GetString("SCALE", resourceCulture);

	internal static string SILENT => ResourceManager.GetString("SILENT", resourceCulture);

	internal static string SKYBOX => ResourceManager.GetString("SKYBOX", resourceCulture);

	internal static string SKYBOX_COLOR => ResourceManager.GetString("SKYBOX_COLOR", resourceCulture);

	internal static string SLAP_SPEED_RANDOMIZER => ResourceManager.GetString("SLAP_SPEED_RANDOMIZER", resourceCulture);

	internal static string SLENDER_BALDI => ResourceManager.GetString("SLENDER_BALDI", resourceCulture);

	internal static string SPEED => ResourceManager.GetString("SPEED", resourceCulture);

	internal static string STORMY_NIGHT => ResourceManager.GetString("STORMY_NIGHT", resourceCulture);

	internal Strings()
	{
	}

	internal static string CAMERA__INDEX => ResourceManager.GetString("CAMERA__INDEX", resourceCulture);

	internal static string REMOVE_CAMERA__INDEX => ResourceManager.GetString("REMOVE_CAMERA__INDEX", resourceCulture);

	internal static string REMOVE_DETENTION_TIME__INDEX => ResourceManager.GetString("REMOVE_DETENTION_TIME__INDEX", resourceCulture);
}
