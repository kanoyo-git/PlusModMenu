using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Hl1;

internal sealed class LooseFall : MonoBehaviour
{
	public float Above;

	private float _speed;

	public static int DropOver(Solid support)
	{
		var count = 0;
		foreach (var t in Loose())
		{
			if (t == null || !t.gameObject.activeInHierarchy || t.GetComponent<Solid>() != null) continue;
			var p = t.position;
			if (p.x < support.MinX || p.x > support.MaxX || p.z < support.MinZ || p.z > support.MaxZ) continue;
			var above = p.y - support.Top;
			if (above < -0.2f || above > 3.5f) continue;
			var fall = t.GetComponent<LooseFall>() ?? t.gameObject.AddComponent<LooseFall>();
			fall.Above = above;
			count++;
		}
		return count;
	}

	private static System.Collections.Generic.IEnumerable<Transform> Loose()
	{
		foreach (var pickup in FindObjectsOfType<Pickup>()) yield return pickup.transform;
		foreach (var notebook in FindObjectsOfType<Notebook>()) yield return notebook.transform;
	}

	private void Update()
	{
		var p = transform.position;
		var target = PropBody.Support(null, p, new Vector3(0.5f, 0f, 0.5f), p.y - Above + 0.01f) + Above;
		if (p.y <= target)
		{
			Destroy(this);
			return;
		}
		_speed += PropBody.Gravity * Time.deltaTime;
		p.y = Mathf.Max(target, p.y - _speed * Time.deltaTime);
		transform.position = p;
		if (p.y <= target) Destroy(this);
	}
}
