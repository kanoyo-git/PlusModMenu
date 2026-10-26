using System.Collections.Generic;
using System.Reflection;
using PlusModMenu.Core.Common.Extensions;
using HarmonyLib;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.CharacterHelpers;

internal class ChalklesHelper : MonoBehaviour
{
	private static readonly FieldInfo _chalkboard = AccessTools.Field(typeof(Chalkboard), "chalkboard");

	private static readonly FieldInfo _functions = AccessTools.Field(typeof(RoomFunctionContainer), "functions");

	private static readonly FieldInfo _hardCoverage = AccessTools.Field(typeof(Cell), "hardCoverage");

	private static readonly FieldInfo _softCoverage = AccessTools.Field(typeof(Cell), "softCoverage");

	private readonly HashSet<(Chalkboard? chalkboard, Cell? tile, Direction direction)> _dependentObjects = new HashSet<(Chalkboard, Cell, Direction)>();

	public void Add(Chalkboard chalkboard, Cell tile, Direction direction)
	{
		_dependentObjects.Add((chalkboard, tile, direction));
	}

	private void OnDisable()
	{
		foreach (var dependentObject in _dependentObjects)
		{
			Chalkboard item = dependentObject.chalkboard;
			if (!(item == null))
			{
				item.enabled = false;
			}
		}
	}

	private void OnEnable()
	{
		foreach (var dependentObject in _dependentObjects)
		{
			Chalkboard item = dependentObject.chalkboard;
			if (!(item == null))
			{
				item.enabled = true;
			}
		}
	}

	private void OnDestroy()
	{
		foreach (var (chalkboard, val, direction) in _dependentObjects)
		{
			if (val != null)
			{
				CellCoverage cellCoverage = (CellCoverage)(1 << (int)direction);
				_hardCoverage.SetValue(val, _hardCoverage.GetValue<CellCoverage>(val) & ~cellCoverage);
				_softCoverage.SetValue(val, _softCoverage.GetValue<CellCoverage>(val) & ~cellCoverage);
			}
			if (chalkboard == null)
			{
				continue;
			}
			RoomController room = chalkboard.Room;
			if (room != null)
			{
				foreach (Door door in room.doors)
				{
					door.Unlock();
				}
				if (room.functions != null)
				{
					_functions.GetValue<List<RoomFunction>>(room.functions)?.Remove(chalkboard);
				}
			}
			Object.Destroy(_chalkboard.GetValue(chalkboard) as GameObject);
			Object.Destroy(chalkboard);
		}
	}
}
