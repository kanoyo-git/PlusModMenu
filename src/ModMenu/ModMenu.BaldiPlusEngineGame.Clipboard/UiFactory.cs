using System;
using System.Linq;
using ModMenu.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ModMenu.BaldiPlusEngineGame.Clipboard;

internal static class UiFactory
{
	public static readonly Color Ink = Color.black;

	public static readonly Color Red = new Color32(0xBD, 0x0F, 0x09, 0xFF);

	public static readonly Color Grey = new Color32(0x80, 0x80, 0x80, 0xFF);

	public static RectTransform Rect(string name, Transform parent)
	{
		var go = new GameObject(name, typeof(RectTransform));
		go.layer = 5;
		var rect = (RectTransform)go.transform;
		rect.SetParent(parent, false);
		return rect;
	}

	public static RectTransform Place(this RectTransform rect, Vector2 position, Vector2 size)
	{
		rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
		rect.pivot = new Vector2(0.5f, 0.5f);
		rect.anchoredPosition = position;
		rect.sizeDelta = size;
		return rect;
	}

	public static RectTransform PlaceTopLeft(this RectTransform rect, float x, float y, float width, float height)
	{
		rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
		rect.pivot = new Vector2(0f, 1f);
		rect.anchoredPosition = new Vector2(x, -y);
		rect.sizeDelta = new Vector2(width, height);
		return rect;
	}

	public static RectTransform Stretch(this RectTransform rect, float left = 0f, float top = 0f, float right = 0f, float bottom = 0f)
	{
		rect.anchorMin = Vector2.zero;
		rect.anchorMax = Vector2.one;
		rect.pivot = new Vector2(0.5f, 0.5f);
		rect.offsetMin = new Vector2(left, bottom);
		rect.offsetMax = new Vector2(0f - right, 0f - top);
		return rect;
	}

	public static Image Image(string name, Transform parent, Sprite sprite, Image.Type type = UnityEngine.UI.Image.Type.Simple)
	{
		var image = Rect(name, parent).gameObject.AddComponent<Image>();
		image.sprite = sprite;
		image.type = type;
		image.raycastTarget = false;
		PixelScale(image);
		image.rectTransform.sizeDelta = sprite != null ? sprite.rect.size : Vector2.zero;
		return image;
	}

	public static void PixelScale(Image image)
	{
		if (image.sprite != null && image.sprite.pixelsPerUnit > 0f)
		{
			image.pixelsPerUnitMultiplier = 100f / image.sprite.pixelsPerUnit;
		}
	}

	public static Image Frame(string name, Transform parent)
	{
		return Image(name, parent, GameAssets.FrameSprite, UnityEngine.UI.Image.Type.Sliced);
	}

	public static Image Fill(string name, Transform parent, Color color)
	{
		var image = Image(name, parent, GameAssets.White);
		image.color = color;
		return image;
	}

	public static TextMeshProUGUI Text(string name, Transform parent, string font, string text, TextAlignmentOptions alignment = TextAlignmentOptions.Left)
	{
		var label = Rect(name, parent).gameObject.AddComponent<TextMeshProUGUI>();
		label.font = GameAssets.Font(font);
		label.fontSharedMaterial = GameAssets.TextMaterial(label.font);
		label.fontSize = FontSize(font);
		label.color = Ink;
		label.alignment = alignment;
		label.enableWordWrapping = false;
		label.overflowMode = TextOverflowModes.Overflow;
		label.raycastTarget = false;
		label.text = text;
		return label;
	}

	public static void Ellipsis(TMP_Text label)
	{
		label.overflowMode = TextOverflowModes.Ellipsis;
		label.margin = new Vector4(0f, -EllipsisSpare, 0f, -EllipsisSpare);
	}

	private const float EllipsisSpare = 8f;

	public static float FontSize(string font)
	{
		return font switch
		{
			AssetNames.Font12 => 12f,
			AssetNames.Font18 => 18f,
			_ => 24f,
		};
	}

	public static Hotspot Clickable(Graphic graphic, Action onClick, Action<bool> hover = null)
	{
		graphic.raycastTarget = true;
		var hotspot = graphic.gameObject.AddComponent<Hotspot>();
		hotspot.Click = onClick;
		hotspot.Hover = hover;
		return hotspot;
	}

	public static TMP_InputField Input(string name, Transform parent, string font, float width, float height)
	{
		var frame = Frame(name, parent);
		frame.raycastTarget = true;
		frame.rectTransform.sizeDelta = new Vector2(width, height);
		var go = frame.gameObject;
		var wasActive = go.activeSelf;
		go.SetActive(false);
		var area = Rect("TextArea", frame.rectTransform);
		area.Stretch(4f, 1f, 4f, 1f);
		area.gameObject.AddComponent<RectMask2D>();
		var text = Text("Text", area, font, string.Empty);
		text.rectTransform.Stretch();
		var placeholder = Text("Placeholder", area, font, string.Empty);
		placeholder.rectTransform.Stretch();
		placeholder.color = Grey;
		var input = go.AddComponent<TMP_InputField>();
		input.transition = Selectable.Transition.None;
		input.targetGraphic = frame;
		input.textViewport = area;
		input.textComponent = text;
		input.placeholder = placeholder;
		input.fontAsset = text.font;
		input.pointSize = text.fontSize;
		text.fontSharedMaterial = placeholder.fontSharedMaterial = GameAssets.TextMaterial(text.font);
		input.lineType = TMP_InputField.LineType.SingleLine;
		input.richText = false;
		input.customCaretColor = true;
		input.caretColor = Ink;
		input.caretWidth = 1;
		input.selectionColor = new Color(Red.r, Red.g, Red.b, 0.35f);
		input.onFocusSelectAll = true;
		input.restoreOriginalTextOnEscape = true;
		TextInputs.Track(input);
		go.SetActive(wasActive);
		return input;
	}

	public static Graphic HitArea(RectTransform rect)
	{
		var image = rect.gameObject.AddComponent<Image>();
		image.color = Color.clear;
		image.raycastTarget = true;
		return image;
	}
}

internal static class TextInputs
{
	private static int _endedFrame = -1;

	public static void Track(TMP_InputField input)
	{
		input.onEndEdit.AddListener(_ => _endedFrame = Time.frameCount);
	}

	public static bool Focused
	{
		get
		{
			if (_endedFrame == Time.frameCount)
			{
				return true;
			}
			var selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
			return selected != null && selected.GetComponent<TMP_InputField>() is { isFocused: true };
		}
	}
}

internal sealed class Hotspot : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	private static readonly Vector3 PressOffset = new Vector3(1f, -1f, 0f);

	private const float PressShade = 0.7f;

	public Action Click;

	public Action<bool> Hover;

	public Action RightClick;

	public RectTransform PressTarget;

	public bool Feedback = true;

	private Graphic _shaded;

	private Color _shadedColor;

	private bool _pressed;

	public bool Hovered { get; private set; }

	public bool Pressed => _pressed;

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right && RightClick != null)
		{
			UiSound.Confirm();
			RightClick();
			return;
		}
		if (eventData.button == PointerEventData.InputButton.Left && Click != null)
		{
			if (Feedback)
			{
				UiSound.Confirm();
			}
			Click();
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left && Click != null && Feedback)
		{
			Press(true);
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			Press(false);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Hovered = true;
		Hover?.Invoke(true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Press(false);
		Hovered = false;
		Hover?.Invoke(false);
	}

	private void OnDisable()
	{
		Press(false);
		if (Hovered)
		{
			Hovered = false;
			Hover?.Invoke(false);
		}
	}

	private void Press(bool down)
	{
		if (down == _pressed)
		{
			return;
		}
		_pressed = down;
		var target = PressTarget != null ? PressTarget : (RectTransform)transform;
		if (down)
		{
			target.localPosition += PressOffset;
			_shaded = target.GetComponent<Graphic>();
			if (_shaded != null && _shaded.color.a > 0f)
			{
				_shadedColor = _shaded.color;
				_shaded.color = new Color(_shadedColor.r * PressShade, _shadedColor.g * PressShade, _shadedColor.b * PressShade, _shadedColor.a);
			}
			else
			{
				_shaded = null;
			}
		}
		else
		{
			if (target != null)
			{
				target.localPosition -= PressOffset;
			}
			if (_shaded != null)
			{
				if (_shaded.color == new Color(_shadedColor.r * PressShade, _shadedColor.g * PressShade, _shadedColor.b * PressShade, _shadedColor.a))
				{
					_shaded.color = _shadedColor;
				}
				_shaded = null;
			}
		}
	}
}

internal static class UiSound
{
	private static AudioSource _source;

	private static SoundObject _confirm;

	public static int Played { get; private set; }

	public static bool Ready => _confirm != null && _confirm.soundClip != null && _source != null;

	public static void Confirm()
	{
		Played++;
		try
		{
			if (_confirm == null)
			{
				var cursor = Resources.FindObjectsOfTypeAll<CursorController>().FirstOrDefault();
				if (cursor != null)
				{
					_confirm = HarmonyLib.Traverse.Create(cursor).Field<SoundObject>("audConfirm").Value;
				}
			}
			if (_confirm == null || _confirm.soundClip == null)
			{
				return;
			}
			if (_source == null)
			{
				var go = new GameObject("Plus Mod Menu Sound");
				UnityEngine.Object.DontDestroyOnLoad(go);
				_source = go.AddComponent<AudioSource>();
				_source.ignoreListenerPause = true;
				_source.spatialBlend = 0f;
				_source.playOnAwake = false;
				var music = Singleton<MusicManager>.Instance;
				var gameSource = music != null ? music.GetComponentsInChildren<AudioSource>(true).FirstOrDefault(s => s.outputAudioMixerGroup != null) : null;
				if (gameSource != null)
				{
					_source.outputAudioMixerGroup = gameSource.outputAudioMixerGroup;
				}
			}
			_source.PlayOneShot(_confirm.soundClip, _confirm.volumeMultiplier);
		}
		catch (Exception e)
		{
			Debug.LogWarning("Plus Mod Menu: couldn't play the click sound: " + e.Message);
		}
	}
}
