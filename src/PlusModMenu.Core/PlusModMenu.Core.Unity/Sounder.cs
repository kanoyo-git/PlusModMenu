using System.Collections;
using UnityEngine;

namespace PlusModMenu.Core.Unity;

public class Sounder
{
	public static AudioSource PlaySoundAtTransform(AudioClip audio, Transform transform)
	{
		GameObject gameObject = new GameObject();
		AudioSource audioSource = gameObject.AddComponent<AudioSource>();
		gameObject.transform.SetParent(transform);
		gameObject.transform.localPosition = Vector3.zero;
		audioSource.ignoreListenerPause = true;
		audioSource.ignoreListenerVolume = true;
		audioSource.clip = audio;
		audioSource.Play();
		new TaskManager.SimpleTask(DeleteObjectAfterPlayback(audioSource)).Start();
		return audioSource;
	}

	private static IEnumerator DeleteObjectAfterPlayback(AudioSource source)
	{
		while ((bool)source && source.isPlaying)
		{
			yield return null;
		}
		if ((bool)source)
		{
			Object.Destroy(source.gameObject);
		}
	}
}
