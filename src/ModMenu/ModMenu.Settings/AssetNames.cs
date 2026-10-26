namespace ModMenu.Settings;

public static class AssetNames
{
	public const string Clipboard = "OptionsClipboard";

	public const string ArrowLeftDim = "MenuArrowSheet_2";

	public const string ArrowRightDim = "MenuArrowSheet_3";

	public const string ArrowLeft = "MenuArrowSheet_0";

	public const string ArrowRight = "MenuArrowSheet_1";

	public const string BarEmpty = "MenuBarSheet_1";

	public const string BarFilled = "MenuBarSheet_0";

	public const string CheckBox = "CheckBox";

	public const string Check = "YCTP_IndicatorsSheet_0";

	public const string Back = "BackArrow_0";

	public const string BackHover = "BackArrow_1";

	public const string Frame = "TooltipBG";

	public const string DisabledCover = "WhiteDitherCover";

	public const string Cursor = "CursorSprite";

	public const string Font12 = "COMIC_12_Pro";

	public const string Font18 = "COMIC_18_Pro";

	public const string Font24 = "COMIC_24_Pro";

	public const string FallbackFont = "LiberationSans SDF";

	public static readonly string[] Sprites = new string[14]
	{
		Clipboard, ArrowLeftDim, ArrowRightDim, ArrowLeft, ArrowRight, BarEmpty, BarFilled, CheckBox, Check, Back, BackHover, Frame, DisabledCover, Cursor
	};

	public static readonly string[] Fonts = new string[3] { Font12, Font18, Font24 };
}
