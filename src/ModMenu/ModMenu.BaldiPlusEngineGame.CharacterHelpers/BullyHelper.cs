using System.Linq;
using ModMenu.BaldiPlusEngineGame.Variables.CharacterModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.CharacterHelpers;

internal class BullyHelper : MonoBehaviour
{
	private static bool IgnoresPlayerTrigger => Service.Get<IModMenuService>().Get<BullyModifiers>().IgnoresPlayerTrigger;

	private void OnTriggerEnter(Collider other)
	{
		if (IgnoresPlayerTrigger && other.CompareTag("Player"))
		{
			Collider collider = GetComponentsInChildren<Collider>().FirstOrDefault((Collider x) => !x.isTrigger);
			if (collider != null)
			{
				collider.enabled = false;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Collider collider = GetComponentsInChildren<Collider>().FirstOrDefault((Collider x) => !x.isTrigger);
			if (collider != null)
			{
				collider.enabled = true;
			}
		}
	}
}
