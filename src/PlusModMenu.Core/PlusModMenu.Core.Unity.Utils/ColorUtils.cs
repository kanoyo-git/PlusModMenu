using System;
using UnityEngine;

namespace PlusModMenu.Core.Unity.Utils;

public class ColorUtils
{
	public static Color ColorFromHTMLString(string argbHtml)
	{
		if (ColorUtility.TryParseHtmlString("#" + argbHtml.TrimStart('#'), out var color))
		{
			return color;
		}
		throw new Exception("Color couldn't be converted from string '" + argbHtml + "'");
	}
}
