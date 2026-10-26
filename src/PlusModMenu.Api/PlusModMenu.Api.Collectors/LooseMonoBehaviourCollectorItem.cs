using System.Reflection;
using UnityEngine;

namespace PlusModMenu.Api.Collectors;

public class LooseMonoBehaviourCollectorItem<TItem> : MonoBehaviourCollectorItem where TItem : MonoBehaviour
{
	internal MethodInfo _internalRemove;

	public TItem Item { get; protected set; }

	internal override void Create(object item)
	{
		Item = (TItem)item;
		CreateInternals(item.GetType(), out MethodInfo internalAdd, out _internalRemove);
		internalAdd.Invoke(null, new object[1] { this });
	}

	private void OnDestroy()
	{
		_internalRemove.Invoke(null, new object[1] { this });
	}
}
