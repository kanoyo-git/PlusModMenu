using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlusModMenu.Core.Unity.Extensions;

public static class TransformEx
{
	public static IEnumerable<Transform> GetChildrenActiveSelf(this Transform @this)
	{
		return from Transform x in @this
			where x.gameObject.activeSelf
			select x;
	}
}
