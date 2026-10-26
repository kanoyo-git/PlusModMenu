using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlusModMenu.Core.Unity;
using PlusModMenu.Core.Unity.Extensions;
using PlusModMenu.Api;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables.OtherModifiers;

internal class VideoDiarrheaManager
{
	private static readonly int _mainTex = Shader.PropertyToID("_MainTex");

	private readonly TaskManager.SimpleTask _task;

	private static readonly int _skybox = Shader.PropertyToID("_Skybox");

	private Texture? _originalSkybox;

	private Cubemap[] _cubemaps = Array.Empty<Cubemap>();

	private List<Renderer> _renderers = new List<Renderer>();

	private Material[] _materials = Array.Empty<Material>();

	private Texture[] _textures = Array.Empty<Texture>();

	private bool _enabled;

	public bool Enabled
	{
		get
		{
			return _enabled;
		}
		set
		{
			_enabled = value;
			if (value)
			{
				_task.Reset();
				_task.Start();
				Service.Get<IEventService>().Subscribe<ILevelStartedEvent>(Reset);
			}
			else
			{
				_task.Stop();
				if (_originalSkybox != null)
				{
					Shader.SetGlobalTexture(_skybox, _originalSkybox);
				}
				Service.Get<IEventService>().Unsubscribe<ILevelStartedEvent>(Reset);
			}
		}
	}

	public float MinCooldown { get; set; }

	public float MaxCooldown { get; set; } = 1f;

	internal VideoDiarrheaManager()
	{
		_task = new TaskManager.SimpleTask(Routine);
	}

	private void Reset(object? _)
	{
		_originalSkybox = Shader.GetGlobalTexture(_skybox);
		_cubemaps = Resources.FindObjectsOfTypeAll<Cubemap>();
		HashSet<Renderer> hashSet = new HashSet<Renderer>();
		HashSet<Material> hashSet2 = new HashSet<Material>();
		HashSet<Texture> hashSet3 = new HashSet<Texture>();
		Material[] materials = _materials;
		for (int i = 0; i < materials.Length; i++)
		{
			UnityEngine.Object.Destroy(materials[i]);
		}
		GameObject[] array = Resources.FindObjectsOfTypeAll<GameObject>();
		foreach (GameObject gameObject in array)
		{
			if (gameObject.scene.IsGameScene() && !(gameObject.GetComponent<TextMesh>() != null) && !(gameObject.GetComponent<Renderer>() == null))
			{
				hashSet.Add(gameObject.GetComponent<Renderer>());
				Material material = gameObject.GetComponent<Renderer>().material;
				hashSet2.Add(material);
				if (material.HasProperty(_mainTex))
				{
					hashSet3.Add(material.mainTexture);
				}
			}
		}
		_renderers = hashSet.ToList();
		_materials = hashSet2.ToArray();
		_textures = hashSet3.ToArray();
	}

	public IEnumerator Routine()
	{
		Reset(null);
		while (true)
		{
			_renderers.RemoveAll((Renderer item) => item == null);
			foreach (Renderer renderer in _renderers)
			{
				renderer.material = _materials[UnityEngine.Random.Range(0, _materials.Length - 1)];
			}
			if (_cubemaps.Length > 0)
			{
				Shader.SetGlobalTexture(_skybox, _cubemaps[UnityEngine.Random.Range(0, _cubemaps.Length)]);
			}
			yield return new WaitForSeconds(UnityEngine.Random.Range(MinCooldown, MaxCooldown));
		}
	}
}
