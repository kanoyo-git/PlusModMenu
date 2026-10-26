namespace ModMenu.Settings;

public static class SearchPath
{
	public static string Format(string section, string page, string group)
	{
		return string.IsNullOrWhiteSpace(group) ? section + " > " + page : page + " > " + group;
	}
}
