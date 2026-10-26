using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame;

internal class DebugOverlay : MonoBehaviour
{
	private static readonly FieldInfo _events = AccessTools.Field(typeof(EnvironmentController), "events");

	private static readonly FieldInfo _eventTimes = AccessTools.Field(typeof(EnvironmentController), "eventTimes");

	private static readonly FieldInfo _anger = AccessTools.Field(typeof(Baldi), "anger");

	private static DebugOverlay? _instance;

	private readonly StringBuilder _text = new StringBuilder();

	private GUIStyle? _style;

	private float _nextUpdate;

	public static bool Shown
	{
		get
		{
			return _instance != null && _instance.enabled;
		}
		set
		{
			if (_instance == null)
			{
				if (!value)
				{
					return;
				}
				_instance = new GameObject("ModMenuDebugOverlay").AddComponent<DebugOverlay>();
				DontDestroyOnLoad(_instance.gameObject);
			}
			_instance.enabled = value;
		}
	}

	public static string Text => _instance != null ? _instance._text.ToString() : string.Empty;

	private void Update()
	{
		if (Time.unscaledTime < _nextUpdate)
		{
			return;
		}
		_nextUpdate = Time.unscaledTime + 0.25f;
		Build();
	}

	private void Build()
	{
		_text.Clear();
		var core = Singleton<CoreGameManager>.Instance;
		var manager = Singleton<BaseGameManager>.Instance;
		var ec = manager != null ? manager.Ec : null;
		if (core == null || ec == null || !manager!.GameReady)
		{
			return;
		}
		var scene = core.sceneObject;
		_text.AppendLine($"Seed {core.Seed()}   {(scene != null ? $"{scene.levelTitle} (#{scene.levelNo}, {scene.name})" : "-")}");
		_text.AppendLine($"Time {Clock(ec.SurpassedGameTime)}   NPCs {ec.Npcs.Count}   Notebooks {manager!.FoundNotebooks}/{manager.NotebookTotal}");
		var player = core.GetPlayer(0);
		if (player != null)
		{
			var position = player.transform.position;
			var cell = ec.CellFromPosition(position);
			_text.AppendLine($"Pos {position.x:0.0}, {position.y:0.0}, {position.z:0.0}");
			if (cell != null && !cell.Null)
			{
				_text.AppendLine($"Cell {cell.position.x},{cell.position.z}  Const {cell.ConstBin}  Nav {cell.NavBin}  Shape {(int)cell.shape}{(cell.offLimits ? "  OFF LIMITS" : "")}{(cell.locked ? "  LOCKED" : "")}");
				_text.AppendLine(cell.room != null ? $"Room {cell.room.name} ({cell.room.category})" : "No room");
			}
		}
		var baldi = ec.GetBaldi();
		if (baldi != null)
		{
			_text.AppendLine($"Baldi anger {(float)_anger.GetValue(baldi):0.00}  speed {baldi.Speed:0.0}");
		}
		if (_events.GetValue(ec) is List<RandomEvent> events && _eventTimes.GetValue(ec) is List<float> times)
		{
			_text.Append("Events");
			if (events.Count == 0)
			{
				_text.Append(" none");
			}
			for (var i = 0; i < events.Count && i < times.Count; i++)
			{
				if (events[i] != null)
				{
					_text.Append($"  {events[i].Type} {Clock(times[i])}{(events[i].Active ? " (now)" : "")}");
				}
			}
			_text.AppendLine();
		}
	}

	private static string Clock(float seconds)
	{
		var total = Mathf.Max(0, Mathf.FloorToInt(seconds));
		return $"{total / 60}:{total % 60:00}";
	}

	private void OnGUI()
	{
		if (_text.Length == 0)
		{
			return;
		}
		_style ??= new GUIStyle(GUI.skin.label) { fontSize = 14, wordWrap = false };
		var content = new GUIContent(_text.ToString());
		var size = _style.CalcSize(content);
		var rect = new Rect(10f, 32f, size.x + 12f, size.y + 8f);
		GUI.Box(rect, GUIContent.none);
		GUI.Label(new Rect(rect.x + 6f, rect.y + 4f, size.x, size.y), content, _style);
	}
}
