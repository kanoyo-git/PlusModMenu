using System;
using System.Collections.Generic;
using System.Linq;
using PlusModMenu.Core.Unity;
using ModMenu.BaldiPlusEngineGame.Clipboard.Rows;
using ModMenu.BaldiPlusEngineGame.Settings;
using ModMenu.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ModMenu.BaldiPlusEngineGame.Clipboard;

internal sealed class ClipboardMenu : MonoBehaviour
{
	public const int SortingOrder = 42;

	private GameObject _canvasObject;

	private RectTransform _canvas;

	private ClipboardFrame _frame;

	private bool _open;

	private int _closedFrame = -1;

	private int _screenWidth;

	private int _screenHeight;

	private readonly Dictionary<SettingsSection, SettingsPage> _lastPage = new Dictionary<SettingsSection, SettingsPage>();

	private HeaderBar _header;

	private RectTransform _body;

	private PageView _pageView;

	private bool _dirty;

	private RectTransform _popupLayer;

	private RectTransform _popup;

	private Action _popupClosed;

	private static readonly IReadOnlyList<SettingRow> NoRows = new SettingRow[0];

	public static SettingsRegistry Registry => SettingsRegistryService.Registry;

	public SettingsSection CurrentSection { get; private set; }

	public SettingsPage CurrentPage { get; private set; }

	public IReadOnlyList<SettingRow> VisibleRows => _results?.Rows ?? _pageView?.Rows ?? NoRows;

	internal PageView PageView => _pageView;

	public Tooltip Tooltip { get; private set; }

	private TMP_InputField _search;

	private SearchResultsView _results;

	private float _searchAt = -1f;

	public const float SearchDelay = 0.15f;

	public string SearchText => _search != null ? _search.text : string.Empty;

	public bool ShowingResults => _results != null;

	public static ClipboardMenu Instance { get; private set; }

	public static bool IsOpen => Instance != null && Instance._open;

	public static bool BlocksGame => Instance != null && (Instance._open || Instance._closedFrame == Time.frameCount);

	public ClipboardLayout Layout { get; private set; }

	internal Vector2Int? ScreenOverride { get; set; }

	internal static Vector2? PointerOverride { get; set; }

	internal static Vector2 Pointer => PointerOverride ?? (Vector2)Input.mousePosition;

	public RectTransform Board => _frame.Rect;

	public event Action LayoutChanged;

	public event Action Opened;

	public event Action Closed;

	public static ClipboardMenu Create(Transform parent)
	{
		var go = new GameObject("PlusModMenu");
		go.transform.SetParent(parent, false);
		return go.AddComponent<ClipboardMenu>();
	}

	public DialogLayer Dialogs { get; private set; }

	private void Awake()
	{
		Instance = this;
		Dialogs = gameObject.AddComponent<DialogLayer>();
		SettingsRegistryService.SetDialogs(Dialogs);
	}

	private void Build()
	{
		GameAssets.Load();
		_canvasObject = new GameObject("Canvas", typeof(RectTransform));
		_canvasObject.layer = 5;
		_canvasObject.transform.SetParent(transform, false);
		var canvas = _canvasObject.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.sortingOrder = SortingOrder;
		canvas.pixelPerfect = false;
		var scaler = _canvasObject.AddComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(ClipboardLayout.MaxWidth, ClipboardLayout.CanvasHeight);
		scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
		scaler.matchWidthOrHeight = 1f;
		scaler.referencePixelsPerUnit = 100f;
		_canvasObject.AddComponent<GraphicRaycaster>();
		_canvas = (RectTransform)_canvasObject.transform;
		var dim = UiFactory.Fill("Dim", _canvas, new Color(0f, 0f, 0f, 0.5f));
		dim.rectTransform.Stretch();
		dim.raycastTarget = true;
		_frame = new ClipboardFrame(_canvas);
		_header = new HeaderBar(Board, _canvas, () => StepSection(-1), () => StepSection(1), Close);
		_search = UiFactory.Input("Search", Board, AssetNames.Font12, Layout.SearchWidth, 22f);
		UpdatePlaceholder();
		_search.onFocusSelectAll = false;
		_search.onValueChanged.AddListener(_ => _searchAt = Time.unscaledTime + SearchDelay);
		_popupLayer = UiFactory.Rect("Popups", _canvas);
		_popupLayer.Stretch();
		Dialogs.Attach(_canvas);
		Tooltip = Tooltip.Create(_canvas);
		HandCursor.Create(_canvas);
		UpdateLayout(force: true);
		Registry.PageChanged += OnPageChanged;
		LayoutChanged += Rebuild;
		Localization.Changed += OnLanguageChanged;
		PlaceSearch();
	}

	private void OnDestroy()
	{
		Localization.Changed -= OnLanguageChanged;
	}

	private void OnLanguageChanged()
	{
		UpdatePlaceholder();
		Rebuild(keepScroll: true);
	}

	private void UpdatePlaceholder()
	{
		((TextMeshProUGUI)_search.placeholder).text = SettingRow.Text(PlusModMenu.Api.Settings.UIText.Resource(typeof(Strings), "SEARCH_PLACEHOLDER"));
	}

	public void ShowSection(int index)
	{
		var sections = Registry.Sections;
		if (sections.Count == 0)
		{
			return;
		}
		var section = sections[(index % sections.Count + sections.Count) % sections.Count];
		var pages = section.Pages;
		if (pages.Count == 0)
		{
			return;
		}
		ShowPage(_lastPage.TryGetValue(section, out var last) && pages.Contains(last) ? last : pages[0]);
	}

	private void StepSection(int delta)
	{
		var sections = Registry.Sections;
		var index = CurrentSection != null ? IndexOf(sections, CurrentSection) : 0;
		ShowSection(index + delta);
	}

	public void ShowPage(SettingsPage page, SettingEntry highlight = null)
	{
		if (page == null)
		{
			return;
		}
		if (_canvasObject == null)
		{
			Build();
		}
		CurrentSection = page.Section;
		CurrentPage = page;
		_lastPage[page.Section] = page;
		Rebuild(keepScroll: false);
		if (highlight != null)
		{
			_pageView?.ScrollTo(highlight)?.Flash();
		}
	}

	private void Rebuild()
	{
		Rebuild(keepScroll: true);
	}

	private void PlaceSearch()
	{
		_search.GetComponent<RectTransform>().Place(new Vector2(0f, 70f), new Vector2(Layout.SearchWidth, 22f));
	}

	public void Search(string query)
	{
		if (_canvasObject == null)
		{
			Build();
		}
		_search.SetTextWithoutNotify(query ?? string.Empty);
		RunSearch();
	}

	public void ClearSearch()
	{
		Search(string.Empty);
	}

	private void RunSearch()
	{
		_searchAt = -1f;
		if (string.IsNullOrWhiteSpace(_search.text))
		{
			if (_results != null)
			{
				_results = null;
				Rebuild(keepScroll: true);
			}
			return;
		}
		Rebuild(keepScroll: true);
	}

	private void OpenResult(SettingEntry entry)
	{
		_search.SetTextWithoutNotify(string.Empty);
		_results = null;
		EventSystem.current?.SetSelectedGameObject(null);
		ShowPage(entry.Group.Page, entry);
	}

	private SettingsSection _listSection;

	private int _listScroll;

	internal int PageListScroll => _listScroll;

	internal void RebuildNow()
	{
		Rebuild(keepScroll: true);
	}

	private void Rebuild(bool keepScroll)
	{
		_dirty = false;
		PlaceSearch();
		if (CurrentPage == null)
		{
			return;
		}
		if (!string.IsNullOrWhiteSpace(_search.text))
		{
			ShowResults();
			return;
		}
		_results = null;
		var scroll = keepScroll && _pageView != null ? _pageView.ScrollOffset : 0f;
		ClosePopup();
		if (_body != null)
		{
			_body.gameObject.SetActive(false);
			Destroy(_body.gameObject);
		}
		_body = UiFactory.Rect("Body", Board);
		_body.Stretch();
		var sections = Registry.Sections;
		var index = IndexOf(sections, CurrentSection);
		_header.Show(Neighbour(sections, index - 1), SettingRow.Text(CurrentSection.Label), Neighbour(sections, index + 1));
		var geometry = new ClipboardGeometry(Layout);
		var pages = CurrentSection.Pages;
		if (_listSection != CurrentSection)
		{
			_listSection = CurrentSection;
			_listScroll = 0;
		}
		if (pages.Count == 1)
		{
			var groups = CurrentPage.Groups.Where(g => g.Entries.Count > 0 && !g.Label.IsEmpty).ToList();
			new PageList(_body, geometry, geometry.TwoColumns ? groups.Select(g => SettingRow.Text(g.Label)).ToList() : new List<string> { SettingRow.Text(CurrentPage.Label) }, geometry.TwoColumns ? -1 : 0, i => _pageView?.ScrollTo(groups[i]), groups: true, _listScroll, first => _listScroll = first);
		}
		else
		{
			new PageList(_body, geometry, pages.Select(p => SettingRow.Text(p.Label)).ToList(), IndexOf(pages, CurrentPage), i => ShowPage(pages[i]), groups: false, _listScroll, first => _listScroll = first);
		}
		_pageView = new PageView(_body, geometry, CurrentPage);
		_pageView.ScrollOffset = scroll;
	}

	private void ShowResults()
	{
		ClosePopup();
		if (_body != null)
		{
			_body.gameObject.SetActive(false);
			Destroy(_body.gameObject);
		}
		_pageView = null;
		_body = UiFactory.Rect("Body", Board);
		_body.Stretch();
		var found = SettingsSearch.Find(Registry.AllEntries, _search.text, text => text.Resolve(),
			text => text.Resolve(Localization.EnglishText));
		_results = new SearchResultsView(_body, new ClipboardGeometry(Layout), found, OpenResult);
	}

	private string Neighbour(IReadOnlyList<SettingsSection> sections, int index)
	{
		return sections.Count < 2 ? string.Empty : SettingRow.Text(sections[(index % sections.Count + sections.Count) % sections.Count].Label);
	}

	private static int IndexOf<T>(IReadOnlyList<T> list, T item) where T : class
	{
		for (var i = 0; i < list.Count; i++)
		{
			if (list[i] == item)
			{
				return i;
			}
		}
		return -1;
	}

	private void OnPageChanged(SettingsPage page)
	{
		if (CurrentPage != null && (page == CurrentPage || page.Section == CurrentSection))
		{
			_dirty = true;
		}
	}

	private void LateUpdate()
	{
		if (!_open)
		{
			return;
		}
		if (_searchAt >= 0f && Time.unscaledTime >= _searchAt)
		{
			RunSearch();
		}
		else if (_dirty)
		{
			Rebuild(keepScroll: true);
		}
	}

	private void TypeIntoSearch()
	{
		var typed = Input.inputString;
		if (string.IsNullOrEmpty(typed) || PopupOpen)
		{
			return;
		}
		var text = string.Empty;
		foreach (var c in typed)
		{
			if (char.IsLetterOrDigit(c))
			{
				text += c;
			}
		}
		if (text.Length == 0)
		{
			return;
		}
		_search.text += text;
		_search.ActivateInputField();
		_search.Select();
		StartCoroutine(CaretToEnd());
	}

	private System.Collections.IEnumerator CaretToEnd()
	{
		yield return null;
		_search.caretPosition = _search.text.Length;
		_search.selectionAnchorPosition = _search.selectionFocusPosition = _search.text.Length;
	}

	public void Open()
	{
		if (_open)
		{
			return;
		}
		if (_canvasObject == null)
		{
			Build();
		}
		EnsureEventSystem();
		_open = true;
		_canvasObject.SetActive(true);
		UpdateLayout(force: false);
		if (CurrentPage == null)
		{
			ShowSection(0);
		}
		else
		{
			Rebuild(keepScroll: true);
		}
		Opened?.Invoke();
		Dialogs.ShowPending();
	}

	public void Close()
	{
		if (!_open)
		{
			return;
		}
		KeyCapture.Cancel();
		ClosePopup();
		Dialogs.CloseAll();
		_open = false;
		_closedFrame = Time.frameCount;
		_canvasObject.SetActive(false);
		Closed?.Invoke();
	}

	public void Toggle()
	{
		if (_open)
		{
			Close();
		}
		else
		{
			Open();
		}
	}

	private void Update()
	{
		if (KeyCapture.IsActive)
		{
			KeyCapture.Update();
			return;
		}
		if (Input.GetKeyDown(Config.Instance.ModMenuKey) && !InputFocused)
		{
			Toggle();
			return;
		}
		if (!_open)
		{
			return;
		}
		UpdateLayout(force: false);
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Escape();
		}
		else if (!InputFocused)
		{
			TypeIntoSearch();
		}
	}

	private void Escape()
	{
		switch (MenuEscape.Next(PopupOpen, InputFocused, SearchHasText))
		{
		case EscAction.ClosePopup:
			if (Dialogs.IsOpen)
			{
				Dialogs.CloseTop();
			}
			else
			{
				ClosePopup();
			}
			break;
		case EscAction.CancelInput:
			EventSystem.current?.SetSelectedGameObject(null);
			break;
		case EscAction.ClearSearch:
			ClearSearch();
			break;
		case EscAction.CloseMenu:
			Close();
			break;
		}
	}

	public bool PopupOpen => _popup != null || Dialogs.IsOpen;

	public RectTransform ShowPopup(string name, Action onClosed = null)
	{
		ClosePopup();
		var blocker = UiFactory.Rect(name, _popupLayer);
		blocker.Stretch();
		UiFactory.Clickable(UiFactory.HitArea(blocker), ClosePopup).Feedback = false;
		_popup = UiFactory.Rect("Content", blocker);
		_popup.Place(Vector2.zero, Vector2.zero);
		_popupClosed = onClosed;
		return _popup;
	}

	public void ClosePopup()
	{
		if (_popup == null)
		{
			return;
		}
		var closed = _popupClosed;
		var blocker = _popup.parent.gameObject;
		_popup = null;
		_popupClosed = null;
		blocker.SetActive(false);
		Destroy(blocker);
		closed?.Invoke();
	}

	public Vector2 PopupPosition(RectTransform target)
	{
		return _popupLayer.InverseTransformPoint(target.TransformPoint(target.rect.center));
	}

	private bool SearchHasText => !string.IsNullOrEmpty(SearchText);

	private static bool InputFocused => TextInputs.Focused;

	private void UpdateLayout(bool force)
	{
		var width = ScreenOverride?.x ?? Screen.width;
		var height = ScreenOverride?.y ?? Screen.height;
		if (!force && width == _screenWidth && height == _screenHeight)
		{
			return;
		}
		_screenWidth = width;
		_screenHeight = height;
		var previous = Layout;
		Layout = ClipboardLayout.For(_screenWidth, _screenHeight);
		_frame.SetWidth(Layout.ClipboardWidth);
		if (!force && (previous.TwoColumns != Layout.TwoColumns || !Mathf.Approximately(previous.ClipboardWidth, Layout.ClipboardWidth)))
		{
			LayoutChanged?.Invoke();
		}
	}

	private static void EnsureEventSystem()
	{
		if (EventSystem.current == null)
		{
			new GameObject("PlusModMenu EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
		}
	}
}
