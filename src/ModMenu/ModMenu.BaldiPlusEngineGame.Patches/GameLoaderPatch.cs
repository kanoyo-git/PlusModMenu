using System.Linq;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class GameLoaderPatch
{
	private static CoreGameManager? _coreGameManagerPrefab;

	public static CoreGameManager? CoreGameManagerPrefab
	{
		get
		{
			if (_coreGameManagerPrefab == null)
			{
				_coreGameManagerPrefab = Resources.FindObjectsOfTypeAll<GameLoader>().Select((GameLoader x) => x.cgmPre).FirstOrDefault((CoreGameManager x) => x != null)
					?? Resources.FindObjectsOfTypeAll<CoreGameManager>().FirstOrDefault((CoreGameManager x) => !x.gameObject.scene.IsValid());
			}
			return _coreGameManagerPrefab;
		}
	}
}
