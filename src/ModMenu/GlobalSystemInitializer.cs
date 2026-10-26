using ModMenu.ReplacementNotice;
using UnityEngine;

internal class GlobalSystemInitializer : MonoBehaviour
{
	private static void LoadMain()
	{
		FullscreenNotice.Create("<color='#ff0000'>ERROR</color>\r\nThe <b>ModMenu</b> requires <b>BepInEx 5</b> to run.\r\n<b>The Assembly-CSharp.dll file-replacement method doesn't work anymore.</b>");
	}
}
