using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Hl1;

internal sealed class CullFollow : MonoBehaviour
{
	private Renderer[] _renderers = new Renderer[0];

	private Cell? _cell;

	private Solid? _solid;

	public static CullFollow Attach(GameObject prop)
	{
		var follow = prop.GetComponent<CullFollow>();
		if (follow != null) return follow;
		follow = prop.AddComponent<CullFollow>();
		follow._solid = prop.GetComponent<Solid>();
		follow.Detach(prop.GetComponentsInChildren<Renderer>(true));
		return follow;
	}

	private void Detach(Renderer[] all)
	{
		var ec = Singleton<BaseGameManager>.Instance != null ? Singleton<BaseGameManager>.Instance.Ec : null;
		if (ec == null || ec.cells == null || all.Length == 0) return;
		var mine = new HashSet<Renderer>(all);
		var culled = new HashSet<Renderer>();
		foreach (var cell in ec.cells)
		{
			if (cell == null || cell.renderers.Count == 0) continue;
			cell.renderers.RemoveAll(r =>
			{
				if (!mine.Contains(r)) return false;
				culled.Add(r);
				return true;
			});
		}
		_renderers = culled.ToArray();
	}

	private void LateUpdate()
	{
		var ec = Singleton<BaseGameManager>.Instance != null ? Singleton<BaseGameManager>.Instance.Ec : null;
		if (ec == null) return;
		var cell = ec.ContainsCoordinates(transform.position) ? ec.CellFromPosition(transform.position) : null;
		if (cell != _cell)
		{
			_cell?.renderers.RemoveAll(r => _renderers.Contains(r));
			if (cell != null) cell.renderers.AddRange(_renderers);
			_cell = cell;
		}
		var shown = (_solid != null && _solid.Held) || _cell == null || _cell.Chunk == null || _cell.Chunk.Rendering;
		foreach (var r in _renderers)
		{
			if (r != null && r.enabled != shown) r.enabled = shown;
		}
	}

	private void OnDestroy() => _cell?.renderers.RemoveAll(r => _renderers.Contains(r));
}
