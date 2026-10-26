using ModMenu.Settings;
using TMPro;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Clipboard.Rows;

internal abstract class SettingRow : MonoBehaviour
{
	public const float Height = 28f;

	public const float LabelX = 30f;

	public const float ControlWidth = 160f;

	private bool _subscribed;

	public SettingEntry Entry { get; private set; }

	public RectTransform Rect => (RectTransform)transform;

	protected TextMeshProUGUI Label { get; private set; }

	public TextMeshProUGUI NameText => Label;

	public System.Action LabelClick { get; set; }

	protected RectTransform Control { get; private set; }

	protected static Vector2 ControlCentre => Vector2.zero;

	public virtual float RowHeight => Height;

	protected virtual float ControlAreaWidth => ControlWidth;

	private UnityEngine.UI.Image _lock;

	private UnityEngine.UI.Image _reset;

	private UnityEngine.UI.Image _cover;

	private UnityEngine.UI.Image _flash;

	public bool ResetShown => _reset != null && _reset.gameObject.activeSelf;

	public bool Covered => _cover != null && _cover.gameObject.activeSelf;

	public static SettingRow Create(SettingEntry entry, RectTransform parent, float width)
	{
		var rect = UiFactory.Rect("Row " + entry.MemberName, parent);
		SettingRow row = entry.Kind switch
		{
			SettingKind.Toggle => rect.gameObject.AddComponent<ToggleRow>(),
			SettingKind.Action => rect.gameObject.AddComponent<ActionRow>(),
			SettingKind.Number => rect.gameObject.AddComponent<NumberRow>(),
			SettingKind.Range => rect.gameObject.AddComponent<RangeRow>(),
			SettingKind.Choice => rect.gameObject.AddComponent<ChoiceRow>(),
			SettingKind.Text => rect.gameObject.AddComponent<TextRow>(),
			SettingKind.Vector2 => rect.gameObject.AddComponent<Vector2Row>(),
			SettingKind.Color => rect.gameObject.AddComponent<ColorRow>(),
			_ => rect.gameObject.AddComponent<PendingRow>(),
		};
		row.Init(entry, width);
		return row;
	}

	protected virtual void Init(SettingEntry entry, float width)
	{
		Entry = entry;
		Rect.anchorMin = Rect.anchorMax = new Vector2(0f, 1f);
		Rect.pivot = new Vector2(0f, 1f);
		Rect.sizeDelta = new Vector2(width, RowHeight);
		_flash = UiFactory.Fill("Flash", Rect, new Color(UiFactory.Red.r, UiFactory.Red.g, UiFactory.Red.b, 0f));
		_flash.rectTransform.Stretch();
		_flash.gameObject.SetActive(false);
		Label = UiFactory.Text("Label", Rect, AssetNames.Font18, Caption());
		var lockWidth = entry.Lock != null ? 20f : 0f;
		Label.rectTransform.PlaceTopLeft(LabelX, 0f, Mathf.Max(20f, width - LabelX - ControlAreaWidth - lockWidth - 6f), RowHeight);
		UiFactory.Ellipsis(Label);
		Control = UiFactory.Rect("Control", Rect);
		Control.anchorMin = Control.anchorMax = new Vector2(1f, 0.5f);
		Control.pivot = new Vector2(1f, 0.5f);
		Control.anchoredPosition = Vector2.zero;
		Control.sizeDelta = new Vector2(ControlAreaWidth, RowHeight);
		if (entry.Lock != null)
		{
			_lock = UiFactory.Image("Lock", Rect, GameAssets.Lock(closed: false));
			_lock.rectTransform.anchorMin = _lock.rectTransform.anchorMax = new Vector2(1f, 0.5f);
			_lock.rectTransform.anchoredPosition = new Vector2(0f - ControlAreaWidth - 10f, 0f);
			_lock.rectTransform.sizeDelta = new Vector2(16f, 16f);
			UiFactory.Clickable(_lock, () => entry.Lock.SetValue(!(entry.Lock.GetValue() is true)));
		}
		Build();
		Label.raycastTarget = true;
		UiFactory.Clickable(Label, () =>
		{
			if (LabelClick != null)
			{
				LabelClick();
			}
			else
			{
				OnLabelClick();
			}
		}, hover => Hover(Label, hover, HelpText));
		if (entry.HasDefault && entry.Setter != null)
		{
			_reset = UiFactory.Image("Reset", Rect, GameAssets.Sprite(AssetNames.Back));
			_reset.rectTransform.PlaceTopLeft(LabelX - 28f - 6f, RowHeight / 2f - 14f, 28f, 28f);
			UiFactory.Clickable(_reset, ResetToDefault, hover =>
			{
				_reset.sprite = GameAssets.Sprite(hover ? AssetNames.BackHover : AssetNames.Back);
				Hover(_reset, hover, () => Text(PlusModMenu.Api.Settings.UIText.Resource(typeof(Strings), "CLICK_TO_RESET", DefaultText())));
			});
		}
		_cover = UiFactory.Image("Unavailable", Rect, GameAssets.Sprite(AssetNames.DisabledCover), UnityEngine.UI.Image.Type.Tiled);
		_cover.rectTransform.Stretch();
		UiFactory.Clickable(_cover, null, hover => Hover(_cover, hover, () => Text(Entry.UnavailableReason)));
		if (SettingBinds.CanBind(entry))
		{
			foreach (var hotspot in GetComponentsInChildren<Hotspot>(includeInactive: true))
			{
				hotspot.RightClick = Bind;
			}
		}
		Subscribe();
		SafeReadValue();
		ReadLock();
		ReadState();
	}

	protected string Caption()
	{
		var text = Text(Entry.Label);
		if (KeptManager.IsKept(Entry))
		{
			text = "<color=#1E8C1E>" + text + "</color>";
		}
		return BindManager.KeyOf(Entry) is KeyCode key ? text + " <color=#808080>[" + key + "]</color>" : text;
	}

	protected virtual void ShowCaption(string text)
	{
		Label.text = text;
	}

	private void Bind()
	{
		BindManager.Capture(Entry, seconds =>
		{
			if (this != null)
			{
				ShowCaption(Text(PlusModMenu.Api.Settings.UIText.Resource(typeof(Strings), "BIND_WAITING__TIME", seconds)));
			}
		}, () =>
		{
			if (this != null)
			{
				ShowCaption(Caption());
			}
			Entry.Label = Entry.Label;
		});
	}

	protected virtual void OnLabelClick()
	{
	}

	public void ResetToDefault()
	{
		if (Entry.Available)
		{
			Entry.ResetToDefault();
		}
	}

	public string HelpText()
	{
		var help = Entry.Help.Resolve();
		if (Entry.HasDefault && Entry.Setter != null && Entry.Kind != SettingKind.Action)
		{
			var line = PlusModMenu.Api.Settings.UIText.Resource(typeof(Strings), "DEFAULT_VALUE", DefaultText()).Resolve();
			help = string.IsNullOrEmpty(help) ? line : help + "\n" + line;
		}
		if (SettingBinds.CanBind(Entry))
		{
			var line = (BindManager.KeyOf(Entry) is KeyCode key ? PlusModMenu.Api.Settings.UIText.Resource(typeof(Strings), "BIND_KEY__KEY", key) : PlusModMenu.Api.Settings.UIText.Resource(typeof(Strings), "BIND_HINT")).Resolve();
			help = string.IsNullOrEmpty(help) ? line : help + "\n" + line;
		}
		if (SettingKeep.CanKeep(Entry))
		{
			var line = PlusModMenu.Api.Settings.UIText.Resource(typeof(Strings), KeptManager.IsKept(Entry) ? "KEPT_HINT" : "KEEP_HINT").Resolve();
			help = string.IsNullOrEmpty(help) ? line : help + "\n" + line;
		}
		if (Label != null && Label.isTextTruncated)
		{
			help = string.IsNullOrEmpty(help) ? Label.text : Label.text + "\n" + help;
		}
		return help;
	}

	private void Hover(object owner, bool hover, System.Func<string> text)
	{
		var tooltip = ClipboardMenu.Instance != null ? ClipboardMenu.Instance.Tooltip : null;
		if (tooltip == null)
		{
			return;
		}
		if (hover)
		{
			tooltip.Request(owner, text);
		}
		else
		{
			tooltip.Cancel(owner);
		}
	}

	private void ReadState()
	{
		if (_reset != null)
		{
			_reset.gameObject.SetActive(Entry.IsModified);
		}
		_cover.gameObject.SetActive(!Entry.Available);
		_cover.transform.SetAsLastSibling();
	}

	public void Flash()
	{
		if (isActiveAndEnabled)
		{
			StartCoroutine(FlashRoutine());
		}
	}

	private System.Collections.IEnumerator FlashRoutine()
	{
		_flash.gameObject.SetActive(true);
		for (var t = 0f; t < 1f; t += Time.unscaledDeltaTime)
		{
			var color = _flash.color;
			color.a = 0.35f * (1f - t) * (Mathf.Sin(t * Mathf.PI * 6f) * 0.5f + 0.5f);
			_flash.color = color;
			yield return null;
		}
		_flash.gameObject.SetActive(false);
	}

	private void ReadLock()
	{
		if (_lock != null)
		{
			_lock.sprite = GameAssets.Lock(Entry.Lock.GetValue() is true);
		}
	}

	protected abstract void Build();

	public abstract void ReadValue();

	private string DefaultText()
	{
		switch (Entry.Default)
		{
		case bool b:
			return b ? Strings.VALUE_ON : Strings.VALUE_OFF;
		case int index when Entry.Kind == SettingKind.Choice:
			var options = Entry.Options?.Invoke();
			return options != null && index >= 0 && index < options.Count ? Text(options[index]) : ValueFormatter.Format(index);
		default:
			return ValueFormatter.Format(Entry.Default);
		}
	}

	public static string Text(PlusModMenu.Api.Settings.UIText text)
	{
		return text.Resolve().Replace("\r", string.Empty).Replace('\n', ' ');
	}

	private void SafeReadValue()
	{
		try
		{
			ReadValue();
		}
		catch (System.Exception e)
		{
			Debug.LogError($"Plus Mod Menu: can't read {Entry.MemberName}: {e}");
		}
	}

	private void OnEnable()
	{
		Subscribe();
		if (Entry != null)
		{
			SafeReadValue();
		}
	}

	private void OnDisable()
	{
		Unsubscribe();
	}

	private void OnDestroy()
	{
		Unsubscribe();
	}

	private void Subscribe()
	{
		if (Entry != null && !_subscribed && isActiveAndEnabled)
		{
			Entry.Changed += OnChanged;
			if (Entry.Lock != null)
			{
				Entry.Lock.Changed += OnChanged;
			}
			_subscribed = true;
		}
	}

	private void Unsubscribe()
	{
		if (_subscribed)
		{
			Entry.Changed -= OnChanged;
			if (Entry.Lock != null)
			{
				Entry.Lock.Changed -= OnChanged;
			}
			_subscribed = false;
		}
	}

	private void OnChanged()
	{
		if (this != null)
		{
			SafeReadValue();
			ReadLock();
			ReadState();
		}
	}
}

internal sealed class PendingRow : SettingRow
{
	private TextMeshProUGUI _value;

	protected override void Build()
	{
		_value = UiFactory.Text("Value", Control, AssetNames.Font18, "…", TextAlignmentOptions.Center);
		_value.rectTransform.Stretch();
	}

	public override void ReadValue()
	{
		_value.text = ValueFormatter.Format(Entry.GetValue());
	}
}
