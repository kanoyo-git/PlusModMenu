using System;
using UnityEngine;

namespace ModMenu.ReplacementNotice;

internal class FullscreenNotice : MonoBehaviour
{
	public string Text { get; set; } = string.Empty;

	public TextAnchor Alignment { get; set; } = TextAnchor.MiddleCenter;

	private FullscreenNotice()
	{
	}

	public static FullscreenNotice Create(string text)
	{
		GameObject[] array = Resources.FindObjectsOfTypeAll<GameObject>();
		foreach (GameObject obj in array)
		{
			try
			{
				UnityEngine.Object.Destroy(obj);
			}
			catch (Exception)
			{
			}
		}
		FullscreenNotice fullscreenNotice = new GameObject(string.Empty).AddComponent<FullscreenNotice>();
		fullscreenNotice.Text = text;
		return fullscreenNotice;
	}

	private void OnGUI()
	{
		GUI.skin.GetStyle("Label").alignment = Alignment;
		GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.blackTexture, ScaleMode.StretchToFill, alphaBlend: false);
		GUI.Label(new Rect(Screen.width / 2 - 320, Screen.height / 2 - 240, 640f, 480f), Text);
	}
}
