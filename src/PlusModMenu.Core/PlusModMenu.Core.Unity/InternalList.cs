using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace PlusModMenu.Core.Unity;

public class InternalList
{
	public struct Scene
	{
		public string Name { get; set; }

		public int BuildIndex { get; set; }

		public string Path { get; set; }
	}

	private static Scene[]? _scenes;

	public static Scene[] Scenes
	{
		get
		{
			if (_scenes != null)
			{
				return _scenes;
			}
			List<Scene> list = new List<Scene>();
			for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
			{
				string scenePathByBuildIndex = SceneUtility.GetScenePathByBuildIndex(i);
				int num = scenePathByBuildIndex.LastIndexOf('/') + 1;
				Scene item = new Scene
				{
					Name = scenePathByBuildIndex.Substring(num, scenePathByBuildIndex.Length - num - 6),
					BuildIndex = i,
					Path = scenePathByBuildIndex
				};
				list.Add(item);
			}
			return _scenes = list.ToArray();
		}
	}
}
