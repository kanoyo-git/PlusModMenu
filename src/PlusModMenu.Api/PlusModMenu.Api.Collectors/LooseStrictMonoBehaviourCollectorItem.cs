using System.Reflection;
using UnityEngine;

namespace PlusModMenu.Api.Collectors;

public class LooseStrictMonoBehaviourCollectorItem<TItem> : MonoBehaviourCollectorItem where TItem : MonoBehaviour
{
	private MethodInfo _internalRemoveLoose;

	private MethodInfo _internalRemoveStrict;

	public TItem Item { get; protected set; }

	internal override void Create(object item)
	{
		Item = (TItem)item;
		CreateInternals(item.GetType(), out MethodInfo internalAdd, out _internalRemoveLoose);
		internalAdd.Invoke(null, new object[1] { this });
		CreateInternals(typeof(TItem), out internalAdd, out _internalRemoveStrict);
		internalAdd.Invoke(null, new object[1] { this });
	}

	private void OnDestroy()
	{
		_internalRemoveLoose.Invoke(null, new object[1] { this });
		_internalRemoveStrict.Invoke(null, new object[1] { this });
	}
}
