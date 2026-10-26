using System;
using System.Globalization;

namespace ModMenu.Settings;

public static class PageListPaging
{
	public const int RowsPerScreen = 8;

	public const string DefaultCaption = "{0}-{1} of {2}";

	public static (int first, int last) Window(int count, int selected, int scroll)
	{
		if (count <= 0)
		{
			return (0, -1);
		}
		var first = scroll;
		if (selected >= 0 && selected < count)
		{
			if (selected < first)
			{
				first = selected;
			}
			else if (selected > first + RowsPerScreen - 1)
			{
				first = selected - RowsPerScreen + 1;
			}
		}
		first = Math.Max(0, Math.Min(first, count - RowsPerScreen));
		return (first, Math.Min(count, first + RowsPerScreen) - 1);
	}

	public static string Caption(int first, int last, int count, string format = DefaultCaption)
	{
		return last < first ? string.Empty : string.Format(CultureInfo.InvariantCulture, format, first + 1, last + 1, count);
	}
}
