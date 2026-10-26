namespace ModMenu.Settings;

public enum MenuLanguage
{
	Auto,
	English,
	Russian
}

public static class LanguageResolver
{
	public const string CyrillicPlusGuid = "blayms.tbb.baldiplus.cyrillic";

	public static (bool russian, bool missingCyrillic) Resolve(MenuLanguage setting, bool cyrillicLoaded)
	{
		switch (setting)
		{
		case MenuLanguage.English:
			return (false, false);
		case MenuLanguage.Russian:
			return (cyrillicLoaded, !cyrillicLoaded);
		default:
			return (cyrillicLoaded, false);
		}
	}
}
