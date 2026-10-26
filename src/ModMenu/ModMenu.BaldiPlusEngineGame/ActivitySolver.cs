using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using ModMenu.BaldiPlusEngineGame.Patches;
using TMPro;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame;

internal static class ActivitySolver
{
	public static bool Solve(Activity? activity)
	{
		if (activity == null || activity.IsCompleted || activity is NoActivity)
		{
			return false;
		}
		var t = Traverse.Create(activity);
		switch (activity)
		{
			case MathMachine machine:
			{
				var total = t.Field<int>("totalProblems").Value;
				t.Field<int>("answeredProblems").Value = total;
				t.Field<bool>("corrupted").Value = false;
				var answer = t.Field<TMP_Text>("answerText").Value;
				if (FortyTwoPatch.On)
				{
					FortyTwoPatch.ShowMachineAnswer(answer);
				}
				else
				{
					answer.text = t.Field<int>("answer").Value.ToString();
				}
				if (total > 1)
				{
					t.Field<TMP_Text>("totalTmp").Value.text = $"<sprite={total}><sprite=10><sprite={total}>";
				}
				machine.Completed(0, correct: true);
				Singleton<BaseGameManager>.Instance.PleaseBaldi(t.Field<float>("baldiPause").Value, rewardSticker: true);
				Win(t);
				return true;
			}
			case BalloonBuster buster:
			{
				if (t.Field<bool>("counting").Value)
				{
					return false;
				}
				var balloons = t.Field<BalloonBusterBalloon[]>("balloon").Value;
				var startingTotal = t.Field<int>("startingTotal").Value;
				for (var i = 0; i < startingTotal && i < balloons.Length; i++)
				{
					if (balloons[i] != null && !balloons[i].popped)
					{
						balloons[i].Pop(silent: false);
					}
				}
				t.Field<TMP_Text>("countTmp").Value.text = FortyTwoPatch.On ? FortyTwoPatch.Answer : t.Field<int>("solution").Value.ToString();
				FinishBuster(buster);
				return true;
			}
			case MatchActivity match:
			{
				var balloons = t.Field<MatchActivityBalloon[]>("balloon").Value;
				var points = Points(t);
				foreach (var balloon in balloons.Where(b => b != null && !b.Completed))
				{
					balloon.Matched();
					if (balloon.MatchingBalloon != null && !balloon.MatchingBalloon.Completed)
					{
						balloon.MatchingBalloon.Matched();
					}
					Singleton<CoreGameManager>.Instance.AddPoints(points, 0, playAnimation: true);
				}
				t.Field<List<MatchActivityBalloon>>("revealedBalloons").Value.Clear();
				match.Completed(0, correct: true);
				Win(t);
				return true;
			}
			default:
				activity.Completed(0, correct: true);
				return true;
		}
	}

	internal static void FinishBuster(BalloonBuster buster)
	{
		var t = Traverse.Create(buster);
		buster.Completed(0, correct: true);
		Win(t);
		Singleton<BaseGameManager>.Instance.PleaseBaldi(t.Field<float>("baldiPause").Value + t.Field<float>("poppedBallonBaldiPauseRate").Value * t.Field<int>("poppedBalloons").Value, rewardSticker: true);
		Singleton<CoreGameManager>.Instance.AddPoints(Points(t), 0, playAnimation: true);
		var player = Singleton<CoreGameManager>.Instance.GetPlayer(0);
		player.plm.AddStamina(player.plm.StaminaMax, limited: true);
		t.Field<bool>("counting").Value = false;
	}

	private static int Points(Traverse t) => t.Field<bool>("bonusMode").Value ? t.Field<int>("bonusPoints").Value : t.Field<int>("normalPoints").Value;

	private static void Win(Traverse t)
	{
		t.Field<AudioManager>("audMan").Value?.PlaySingle(t.Field<SoundObject>("audWin").Value);
	}

	public static int SolveAll()
	{
		var ec = Singleton<BaseGameManager>.Instance?.Ec;
		return ec == null ? 0 : ec.activities.ToList().Count(Solve);
	}
}
