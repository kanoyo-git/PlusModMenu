using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Variables.OtherModifiers;
using PlusModMenu.Api;
using PlusModMenu.Api.Interfaces.Services;
using TMPro;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Patches;

internal class FortyTwoPatch
{
	public const string Answer = "42";

	internal static void ShowMachineAnswer(TMP_Text answer)
	{
		answer.enableWordWrapping = false;
		answer.text = "<size=55%>" + Answer + "</size>";
	}

	internal static bool On => Service.Get<IModMenuService>().Get<OtherModifiers>().FourtyTwo;

	private static Sprite? _sprite;

	private static readonly Dictionary<SpriteRenderer, Sprite> _original = new();

	internal static Sprite? Sprite
	{
		get
		{
			if (_sprite == null)
			{
				var four = Digit(4);
				var two = Digit(2);
				if (four != null && two != null)
				{
					_sprite = Combine(four, two);
				}
			}
			return _sprite;
		}
	}

	private static Sprite? Digit(int value)
	{
		var number = Resources.FindObjectsOfTypeAll<MathMachineNumber>().FirstOrDefault(n => n.Value == value);
		var renderer = number != null ? number.GetComponentsInChildren<SpriteRenderer>(true).FirstOrDefault(r => r.sprite != null) : null;
		return renderer != null ? (_original.TryGetValue(renderer, out var original) ? original : renderer.sprite) : null;
	}

	private static Sprite Combine(Sprite left, Sprite right)
	{
		var a = Pixels(left);
		var b = Pixels(right);
		var w = (int)left.textureRect.width;
		var h = (int)left.textureRect.height;
		var shift = w * 3 / 4;
		var width = shift + w;
		var pixels = new Color[width * h];
		for (var y = 0; y < h; y++)
		{
			for (var x = 0; x < w; x++)
			{
				pixels[y * width + x] = a[y * w + x];
			}
			for (var x = 0; x < w; x++)
			{
				var under = pixels[y * width + x + shift];
				var over = b[y * w + x];
				var alpha = over.a + under.a * (1 - over.a);
				pixels[y * width + x + shift] = alpha <= 0f ? Color.clear : new Color(
					(over.r * over.a + under.r * under.a * (1 - over.a)) / alpha,
					(over.g * over.a + under.g * under.a * (1 - over.a)) / alpha,
					(over.b * over.a + under.b * under.a * (1 - over.a)) / alpha,
					alpha);
			}
		}
		var texture = new Texture2D(width, h, TextureFormat.RGBA32, false) { filterMode = left.texture.filterMode, name = "Balloon42" };
		texture.SetPixels(pixels);
		texture.Apply();
		var pivot = new Vector2(left.pivot.x / width + (float)shift / width / 2, left.pivot.y / h);
		var sprite = UnityEngine.Sprite.Create(texture, new Rect(0, 0, width, h), pivot, left.pixelsPerUnit);
		sprite.name = "NumBall_42";
		return sprite;
	}

	internal static Color[] Pixels(Sprite sprite)
	{
		var rect = sprite.textureRect;
		var rt = RenderTexture.GetTemporary(sprite.texture.width, sprite.texture.height, 0, RenderTextureFormat.ARGB32);
		Graphics.Blit(sprite.texture, rt);
		var previous = RenderTexture.active;
		RenderTexture.active = rt;
		var copy = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);
		copy.ReadPixels(new Rect(rect.x, rect.y, rect.width, rect.height), 0, 0);
		copy.Apply();
		RenderTexture.active = previous;
		RenderTexture.ReleaseTemporary(rt);
		var pixels = copy.GetPixels();
		Object.Destroy(copy);
		return pixels;
	}

	[HarmonyPatch(typeof(MathMachineNumber), "Update")]
	private class NumberBalloon
	{
		private static void Postfix(Transform ___sprite)
		{
			if (___sprite == null || ___sprite.GetComponent<SpriteRenderer>() is not SpriteRenderer renderer)
			{
				return;
			}
			if (On)
			{
				var sprite = Sprite;
				if (sprite != null && renderer.sprite != sprite)
				{
					_original[renderer] = renderer.sprite;
					renderer.sprite = sprite;
				}
			}
			else if (_original.TryGetValue(renderer, out var original))
			{
				renderer.sprite = original;
				_original.Remove(renderer);
			}
		}
	}

	[HarmonyPatch(typeof(MathMachine), "Clicked")]
	private class MathMachineClicked
	{
		private static void Prefix(bool ___powered, int player, bool[] ___playerIsHolding, int[] ___playerHolding, ref int ___answer, ref bool ___corrupted, out bool __state)
		{
			__state = false;
			if (On && ___powered && ___playerIsHolding[player])
			{
				___answer = ___playerHolding[player];
				___corrupted = false;
				__state = true;
			}
		}

		private static void Postfix(TMP_Text ___answerText, bool __state, int ___answeredProblems, int ___totalProblems)
		{
			if (__state && ___answeredProblems >= ___totalProblems)
			{
				ShowMachineAnswer(___answerText);
			}
		}
	}

	[HarmonyPatch(typeof(BalloonBuster), nameof(BalloonBuster.ReInit))]
	private class BalloonBusterTarget
	{
		private static void Postfix(TMP_Text ___solutionTmp)
		{
			if (On)
			{
				___solutionTmp.text = Answer;
			}
		}
	}

	[HarmonyPatch(typeof(BalloonBuster), nameof(BalloonBuster.SubmitAnswer))]
	private class BalloonBusterSubmit
	{
		private static bool Prefix(BalloonBuster __instance, bool ___completed, ref bool ___counting, BalloonBusterBalloon[] ___balloon, int ___startingTotal, ref int ___solution, TMP_Text ___solutionTmp, TMP_Text ___countTmp)
		{
			if (!On || ___completed || ___counting)
			{
				return true;
			}
			var left = new List<BalloonBusterBalloon>();
			for (var i = 0; i < ___startingTotal; i++)
			{
				if (!___balloon[i].popped)
				{
					___balloon[i].Disable();
					left.Add(___balloon[i]);
				}
			}
			___solution = left.Count;
			___solutionTmp.text = Answer;
			___counting = true;
			__instance.StartCoroutine(CountTo42(__instance, left, ___countTmp));
			return false;
		}

		private static IEnumerator CountTo42(BalloonBuster buster, List<BalloonBusterBalloon> left, TMP_Text count)
		{
			const int total = 42;
			var popped = 0;
			count.text = "0";
			for (var shown = 1; shown <= total; shown++)
			{
				var wait = 0.05f;
				while (wait > 0f)
				{
					wait -= Time.deltaTime;
					yield return null;
				}
				while (popped < left.Count && popped < Mathf.CeilToInt((float)shown * left.Count / total))
				{
					left[popped++].Pop(silent: false);
				}
				count.text = shown.ToString();
			}
			ActivitySolver.FinishBuster(buster);
		}
	}

	[HarmonyPatch(typeof(MatchActivity), "BalloonRevealed")]
	private class MatchRevealed
	{
		private static readonly AccessTools.FieldRef<MatchActivityBalloon, MatchActivityBalloon> Matching = AccessTools.FieldRefAccess<MatchActivityBalloon, MatchActivityBalloon>("matchingBalloon");

		private static readonly AccessTools.FieldRef<MatchActivityBalloon, Sprite> Revealed = AccessTools.FieldRefAccess<MatchActivityBalloon, Sprite>("revealedSprite");

		private static void Prefix(MatchActivityBalloon revealedBalloon, bool ___completed, List<MatchActivityBalloon> ___revealedBalloons)
		{
			if (!On)
			{
				return;
			}
			if (Sprite is Sprite sprite)
			{
				Revealed(revealedBalloon) = sprite;
			}
			if (___completed || ___revealedBalloons.Count != 1)
			{
				return;
			}
			var first = ___revealedBalloons[0];
			if (first == revealedBalloon || first.MatchingBalloon == revealedBalloon)
			{
				return;
			}
			var firstPartner = first.MatchingBalloon;
			var secondPartner = revealedBalloon.MatchingBalloon;
			Matching(first) = revealedBalloon;
			Matching(revealedBalloon) = first;
			if (firstPartner != null && secondPartner != null)
			{
				Matching(firstPartner) = secondPartner;
				Matching(secondPartner) = firstPartner;
				Revealed(firstPartner) = Revealed(secondPartner);
			}
		}
	}
}
