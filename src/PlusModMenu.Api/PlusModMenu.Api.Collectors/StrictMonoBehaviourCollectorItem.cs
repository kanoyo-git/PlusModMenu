using System.Reflection;
using UnityEngine;

namespace PlusModMenu.Api.Collectors;

public class StrictMonoBehaviourCollectorItem<TItem> : LooseMonoBehaviourCollectorItem<TItem> where TItem : MonoBehaviour
{
	internal override void Create(object item)
	{
		Item = (TItem)item;
		CreateInternals(typeof(TItem), out MethodInfo internalAdd, out _internalRemove);
		internalAdd.Invoke(null, new object[1] { this });
	}
}
