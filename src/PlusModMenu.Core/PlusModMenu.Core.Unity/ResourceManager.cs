using System.Collections.Generic;
using System.IO;
using System.Reflection;
using PlusModMenu.Core.Common;
using UnityEngine;
using UnityEngine.Networking;

namespace PlusModMenu.Core.Unity;

public class ResourceManager : PlusModMenu.Core.Common.ResourceManager
{
	public class AssetBundle
	{
		private static readonly Dictionary<string, object> _existingAssets = new Dictionary<string, object>();

		public static T? Get<T>(string assetBundleName, string assetName)
		{
			if (Platform.IsAndroid)
			{
				assetBundleName += "_android";
			}
			else if (Platform.IsIOS)
			{
				assetBundleName += "_ios";
			}
			if (_existingAssets.ContainsKey(assetBundleName + "_" + assetName))
			{
				return (T)_existingAssets[assetBundleName + "_" + assetName];
			}
			UnityEngine.AssetBundle assetBundle = UnityEngine.AssetBundle.LoadFromMemory(PlusModMenu.Core.Common.ResourceManager.GetBytes(assetBundleName, Assembly.GetCallingAssembly()));
			if (assetBundle == null)
			{
				return default;
			}
			object obj = assetBundle.LoadAsset(assetName);
			assetBundle.Unload(unloadAllLoadedObjects: false);
			_existingAssets.Add(assetBundleName + "_" + assetName, obj);
			return (T)obj;
		}
	}

	public static AudioClip? Get(string resourceName)
	{
		byte[] bytes = PlusModMenu.Core.Common.ResourceManager.GetBytes(resourceName, Assembly.GetCallingAssembly());
		string text = Application.temporaryCachePath + "/" + resourceName.Replace('/', '_');
		using (FileStream fileStream = new FileStream(text, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
		{
			if (fileStream.Length == 0L)
			{
				fileStream.Write(bytes, 0, bytes.Length);
			}
		}
		using UnityWebRequest unityWebRequest = UnityWebRequestMultimedia.GetAudioClip("file://" + text, AudioType.OGGVORBIS);
		unityWebRequest.SendWebRequest();
		while (!unityWebRequest.isDone)
		{
		}
		if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError)
		{
			Debug.LogError($"{unityWebRequest.responseCode}:\n{unityWebRequest.error}");
			return null;
		}
		return DownloadHandlerAudioClip.GetContent(unityWebRequest);
	}
}
