using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;

namespace PlusModMenu.Core.Unity.Extensions;

public static class SceneEx
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsGameScene(this Scene @this)
	{
		if (@this != default(Scene))
		{
			if (@this.buildIndex > -1)
			{
				return @this.name != null;
			}
			return false;
		}
		return false;
	}
}
