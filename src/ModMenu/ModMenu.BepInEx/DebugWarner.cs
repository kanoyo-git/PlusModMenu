using UnityEngine;

namespace ModMenu.BepInEx;

internal class DebugWarner : MonoBehaviour
{
	private void OnGUI()
	{
		GUIContent content = new GUIContent("<b>YOU ARE RUNNING THE MODMENU WITH A DEBUG VERSION OF THE ASSEMBLY-CSHARP DLL. NOT ALL PATCHES WILL WORK PROPERLY!</b>");
		Vector2 size = GUI.skin.label.CalcSize(content);
		Rect position = new Rect(Vector2.zero, size);
		GUI.DrawTexture(position, Texture2D.blackTexture, ScaleMode.StretchToFill, alphaBlend: false);
		GUI.Label(position, content);
	}
}
