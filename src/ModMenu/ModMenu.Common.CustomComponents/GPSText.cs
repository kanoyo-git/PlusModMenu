using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ModMenu.Common.CustomComponents;

[RequireComponent(typeof(BillboardUpdater), typeof(TextMesh))]
public class GPSText : MonoBehaviour
{
	private const int OFFSET_COUNT = 8;

	private static readonly Vector3[] _offsets = new Vector3[8]
	{
		new Vector3(0f, 1f),
		new Vector3(1f, 1f),
		new Vector3(1f, 0f),
		new Vector3(1f, -1f),
		new Vector3(0f, -1f),
		new Vector3(-1f, -1f),
		new Vector3(-1f, 0f),
		new Vector3(-1f, 1f)
	};

	private TextMesh _mainTextMesh;

	private MeshRenderer _meshRenderer;

	private TextMesh[] _outlineMeshes;

	public static bool ShowDistance { get; set; }

	public static Transform? CalcDistanceFrom { get; set; }

	public string Text { get; set; } = string.Empty;

	public bool Active
	{
		set
		{
			gameObject.SetActive(value);
		}
	}

	public Color MainColor
	{
		get
		{
			return _mainTextMesh.color;
		}
		set
		{
			_mainTextMesh.color = value;
		}
	}

	public Color OutlineColor
	{
		get
		{
			return _outlineMeshes[0].color;
		}
		set
		{
			TextMesh[] outlineMeshes = _outlineMeshes;
			for (int i = 0; i < outlineMeshes.Length; i++)
			{
				outlineMeshes[i].color = value;
			}
		}
	}

	public TextAnchor Anchor
	{
		get
		{
			return _mainTextMesh.anchor;
		}
		set
		{
			_mainTextMesh.anchor = value;
			TextMesh[] outlineMeshes = _outlineMeshes;
			for (int i = 0; i < outlineMeshes.Length; i++)
			{
				outlineMeshes[i].anchor = value;
			}
		}
	}

	public float CharacterSize
	{
		get
		{
			return _mainTextMesh.characterSize;
		}
		set
		{
			_mainTextMesh.characterSize = value;
			TextMesh[] outlineMeshes = _outlineMeshes;
			for (int i = 0; i < outlineMeshes.Length; i++)
			{
				outlineMeshes[i].characterSize = value;
			}
		}
	}

	public int FontSize
	{
		get
		{
			return _mainTextMesh.fontSize;
		}
		set
		{
			_mainTextMesh.fontSize = value;
			TextMesh[] outlineMeshes = _outlineMeshes;
			for (int i = 0; i < outlineMeshes.Length; i++)
			{
				outlineMeshes[i].fontSize = value;
			}
		}
	}

	public TextAlignment Alignment
	{
		get
		{
			return _mainTextMesh.alignment;
		}
		set
		{
			_mainTextMesh.alignment = value;
			TextMesh[] outlineMeshes = _outlineMeshes;
			for (int i = 0; i < outlineMeshes.Length; i++)
			{
				outlineMeshes[i].alignment = value;
			}
		}
	}

	protected void Awake()
	{
		_mainTextMesh = base.gameObject.GetComponent<TextMesh>();
		_meshRenderer = base.gameObject.GetComponent<MeshRenderer>();
		_meshRenderer.allowOcclusionWhenDynamic = false;
		List<TextMesh> list = new List<TextMesh>();
		for (int i = 0; i < 8; i++)
		{
			GameObject gameObject = new GameObject("Outline");
			gameObject.transform.parent = transform;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.transform.rotation = transform.rotation;
			GameObject gameObject2 = gameObject;
			list.Add(gameObject2.AddComponent<TextMesh>());
			gameObject2.AddComponent<SortingGroup>().sortingOrder = -1;
			MeshRenderer component = gameObject2.GetComponent<MeshRenderer>();
			component.material = new Material(_meshRenderer.material);
			component.shadowCastingMode = ShadowCastingMode.Off;
			component.receiveShadows = false;
			component.sortingLayerID = _meshRenderer.sortingLayerID;
			component.sortingLayerName = _meshRenderer.sortingLayerName;
		}
		_outlineMeshes = list.ToArray();
		Anchor = TextAnchor.MiddleCenter;
		CharacterSize = 0.03f;
		FontSize = 355;
		Alignment = TextAlignment.Center;
		MainColor = Color.white;
		OutlineColor = Color.black;
	}

	private void LateUpdate()
	{
		if (!(CalcDistanceFrom == null) && !(Camera.main == null))
		{
			_mainTextMesh.text = Text;
			if (ShowDistance)
			{
				_mainTextMesh.text += $"\n({Mathf.RoundToInt(Vector3.Distance(transform.parent.position, CalcDistanceFrom.position) / 5f)}m)";
			}
			Vector3 vector = Camera.main.WorldToScreenPoint(transform.position);
			for (int i = 0; i < 8; i++)
			{
				Vector3 position = Camera.main.ScreenToWorldPoint(vector + _offsets[i]);
				TextMesh component = _outlineMeshes[i].GetComponent<TextMesh>();
				component.text = _mainTextMesh.text;
				component.transform.position = position;
				MeshRenderer component2 = _outlineMeshes[i].GetComponent<MeshRenderer>();
				component2.sortingLayerID = _meshRenderer.sortingLayerID;
				component2.sortingLayerName = _meshRenderer.sortingLayerName;
			}
		}
	}
}
