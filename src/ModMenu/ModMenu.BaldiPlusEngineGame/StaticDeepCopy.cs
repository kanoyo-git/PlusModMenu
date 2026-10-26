using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame;

internal class StaticDeepCopy
{
	private static readonly GameObject _stackHolder;

	static StaticDeepCopy()
	{
		_stackHolder = new GameObject();
		_stackHolder.SetActive(value: false);
		Object.DontDestroyOnLoad(_stackHolder);
	}

	public static T Create<T>(T original) where T : MonoBehaviour
	{
		return Object.Instantiate(original.gameObject, _stackHolder.transform).GetComponent<T>();
	}
}
