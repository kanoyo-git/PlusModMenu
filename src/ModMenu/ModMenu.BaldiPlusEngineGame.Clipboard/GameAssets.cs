using System.Collections.Generic;
using System.Linq;
using ModMenu.Settings;
using TMPro;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Clipboard;

internal static class GameAssets
{
	private static readonly Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();

	private static readonly Dictionary<string, TMP_FontAsset> Fonts = new Dictionary<string, TMP_FontAsset>();

	private static readonly List<string> MissingList = new List<string>();

	private static Sprite _white;

	private static Sprite _frame;

	private static Sprite _lockOpen;

	private static Sprite _lockClosed;

	private static TMP_FontAsset _fallbackFont;

	private static bool _loaded;

	public static IReadOnlyList<string> Missing => MissingList;

	public static void Load()
	{
		if (_loaded)
		{
			return;
		}
		_loaded = true;
		foreach (var sprite in Resources.FindObjectsOfTypeAll<Sprite>())
		{
			if (AssetNames.Sprites.Contains(sprite.name) && !Sprites.ContainsKey(sprite.name))
			{
				Sprites[sprite.name] = sprite;
			}
		}
		var fonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
		foreach (var font in fonts)
		{
			if (AssetNames.Fonts.Contains(font.name) && !Fonts.ContainsKey(font.name))
			{
				Fonts[font.name] = font;
			}
		}
		_fallbackFont = fonts.FirstOrDefault(f => f.name == AssetNames.FallbackFont) ?? TMP_Settings.defaultFontAsset;
		MissingList.AddRange(AssetNames.Sprites.Where(n => !Sprites.ContainsKey(n)).Concat(AssetNames.Fonts.Where(n => !Fonts.ContainsKey(n))));
		if (MissingList.Count > 0)
		{
			Debug.LogError("Plus Mod Menu: game assets not found, using fallbacks: " + string.Join(", ", MissingList));
		}
	}

	public static Sprite Sprite(string name)
	{
		Load();
		return Sprites.TryGetValue(name, out var sprite) ? sprite : White;
	}

	public static TMP_FontAsset Font(string name)
	{
		Load();
		return Fonts.TryGetValue(name, out var font) ? font : _fallbackFont;
	}

	private static readonly Dictionary<TMP_FontAsset, Material> TextMaterials = new Dictionary<TMP_FontAsset, Material>();

	public static Material TextMaterial(TMP_FontAsset font)
	{
		if (!TextMaterials.TryGetValue(font, out var material) || material == null || material.mainTexture != font.material.mainTexture)
		{
			TextMaterials[font] = material = new Material(font.material) { name = font.material.name + " (Plus Mod Menu)" };
		}
		return material;
	}

	public static Sprite FrameSprite
	{
		get
		{
			if (_frame == null)
			{
				var source = Sprite(AssetNames.Frame);
				_frame = source == White ? White : UnityEngine.Sprite.Create(source.texture, source.textureRect, new Vector2(0.5f, 0.5f), source.pixelsPerUnit, 0u, SpriteMeshType.FullRect, new Vector4(3f, 3f, 3f, 3f));
			}
			return _frame;
		}
	}

	public static Sprite Lock(bool closed)
	{
		if (closed)
		{
			return _lockClosed ??= ContentSprite("lock_closed_16.png");
		}
		return _lockOpen ??= ContentSprite("lock_open_16.png");
	}

	private static Sprite ContentSprite(string name)
	{
		var bytes = PlusModMenu.Core.Common.ResourceManager.GetBytes(name, typeof(GameAssets).Assembly);
		if (bytes.Length == 0)
		{
			return White;
		}
		var texture = new Texture2D(2, 2, TextureFormat.RGBA32, mipChain: false)
		{
			filterMode = FilterMode.Point,
			wrapMode = TextureWrapMode.Clamp
		};
		texture.LoadImage(bytes);
		return UnityEngine.Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.FullRect);
	}

	public static Sprite White => _white ??= UnityEngine.Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height), new Vector2(0.5f, 0.5f), 1f);
}
