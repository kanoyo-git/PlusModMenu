using System;
using PlusModMenu.Core.Common;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Clipboard;
using ModMenu.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModMenu.BaldiPlusEngineGame.Hl1;

internal sealed class Speedometer : MonoBehaviour
{
	public const float FullScale = 1000f;

	private static readonly AccessTools.FieldRef<HudManager, RectTransform> Needle = AccessTools.FieldRefAccess<HudManager, RectTransform>("staminaNeedle");

	private static Sprite? _overlay;

	private GameObject _staminometer = null!;

	private Image? _bar;

	private TextMeshProUGUI[] _numbers = new TextMeshProUGUI[0];

	private Func<float> _speed = () => 0f;

	public string Text => _numbers.Length > 0 ? _numbers[_numbers.Length - 1].text : "";

	public static Speedometer? Create(PlayerManager player, Func<float> speed)
	{
		var hud = Singleton<CoreGameManager>.Instance.GetHud(player.playerNumber);
		var needle = hud != null ? Needle(hud) : null;
		if (needle == null || needle.parent == null)
		{
			Debugger.Warning("Speedometer: the staminometer couldn't be found.");
			return null;
		}
		var staminometer = needle.parent.gameObject;
		var copy = Instantiate(staminometer, staminometer.transform.parent);
		copy.name = "Speedometer";
		copy.transform.SetSiblingIndex(staminometer.transform.GetSiblingIndex() + 1);
		var speedometer = copy.AddComponent<Speedometer>();
		speedometer._staminometer = staminometer;
		speedometer._speed = speed;
		speedometer.Build(needle.name);
		staminometer.SetActive(false);
		copy.SetActive(true);
		return speedometer;
	}

	public void Remove()
	{
		if (_staminometer != null)
		{
			_staminometer.SetActive(true);
		}
		Destroy(gameObject);
	}

	private void Build(string needleName)
	{
		Image? overlay = null;
		foreach (var image in GetComponentsInChildren<Image>(true))
		{
			if (image.name == needleName)
			{
				image.gameObject.SetActive(false);
			}
			else if (image.sprite != null && image.sprite.name.EndsWith("_Overlay"))
			{
				overlay = image;
			}
			else if (image.sprite != null && image.sprite.name.EndsWith("_BG"))
			{
				_bar = image;
			}
		}
		foreach (Transform child in transform)
		{
			if (child.name == needleName) child.gameObject.SetActive(false);
		}
		if (_bar != null)
		{
			_bar.type = Image.Type.Filled;
			_bar.fillMethod = Image.FillMethod.Horizontal;
			_bar.fillOrigin = (int)Image.OriginHorizontal.Left;
			_bar.fillAmount = 0;
			_numbers = Number(_bar.rectTransform);
		}
		if (overlay != null)
		{
			_overlay ??= EraseCaption(overlay.sprite);
			if (_overlay != null)
			{
				overlay.sprite = _overlay;
			}
			var label = UiFactory.Text("Caption", overlay.transform, AssetNames.Font12, "SPEEDOMETER", TextAlignmentOptions.Top);
			label.rectTransform.anchorMin = new Vector2(0, 1);
			label.rectTransform.anchorMax = new Vector2(1, 1);
			label.rectTransform.pivot = new Vector2(0.5f, 1);
			label.rectTransform.sizeDelta = new Vector2(0, 16);
			label.rectTransform.anchoredPosition = new Vector2(0, -1);
		}
	}

	private static TextMeshProUGUI[] Number(RectTransform bar)
	{
		var offsets = new[] { new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, -1), new Vector2(0, 1), Vector2.zero };
		var numbers = new TextMeshProUGUI[offsets.Length];
		for (var i = 0; i < offsets.Length; i++)
		{
			var text = UiFactory.Text("Speed", bar, AssetNames.Font24, "0", TextAlignmentOptions.Center);
			text.rectTransform.Stretch();
			text.rectTransform.anchoredPosition = offsets[i];
			text.color = i < offsets.Length - 1 ? Color.white : UiFactory.Ink;
			numbers[i] = text;
		}
		return numbers;
	}

	private void Update()
	{
		var speed = _speed();
		if (_bar != null)
		{
			_bar.fillAmount = Mathf.Clamp01(speed / FullScale);
		}
		var text = Mathf.RoundToInt(speed).ToString();
		foreach (var number in _numbers)
		{
			number.text = text;
		}
	}

	private static Sprite? EraseCaption(Sprite source)
	{
		try
		{
			var rect = source.textureRect;
			var width = (int)rect.width;
			var height = (int)rect.height;
			var target = RenderTexture.GetTemporary(source.texture.width, source.texture.height, 0, RenderTextureFormat.ARGB32);
			var previous = RenderTexture.active;
			Graphics.Blit(source.texture, target);
			RenderTexture.active = target;
			var copy = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false) { filterMode = FilterMode.Point };
			copy.ReadPixels(new Rect(rect.x, rect.y, width, height), 0, 0);
			RenderTexture.active = previous;
			RenderTexture.ReleaseTemporary(target);
			var pixels = copy.GetPixels32();
			for (var top = 3; top <= 16 && top < height; top++)
			{
				var y = height - 1 - top;
				var reference = Luminance(pixels[y * width + Math.Min(165, width - 1)]) * 0.9f;
				for (var x = 26; x < Math.Min(156, width); x++)
				{
					var p = pixels[y * width + x];
					if (p.a == 0 || Luminance(p) >= reference && IsGreen(p)) continue;
					for (var d = 1; d < 60; d++)
					{
						if (Plain(pixels, width, x - d, y, reference, out var g) || Plain(pixels, width, x + d, y, reference, out g))
						{
							pixels[y * width + x] = g;
							break;
						}
					}
				}
			}
			copy.SetPixels32(pixels);
			copy.Apply();
			return Sprite.Create(copy, new Rect(0, 0, width, height), new Vector2(source.pivot.x / width, source.pivot.y / height), source.pixelsPerUnit);
		}
		catch (Exception e)
		{
			Debugger.Warning("Speedometer: couldn't redraw the staminometer caption: " + e.Message);
			return null;
		}
	}

	private static bool Plain(Color32[] pixels, int width, int x, int y, float reference, out Color32 color)
	{
		color = default;
		if (x < 0 || x >= width) return false;
		color = pixels[y * width + x];
		return color.a > 0 && IsGreen(color) && Luminance(color) >= reference;
	}

	private static bool IsGreen(Color32 c) => c.g > 120 && c.r < 200;

	private static float Luminance(Color32 c) => 0.3f * c.r + 0.59f * c.g + 0.11f * c.b;
}
