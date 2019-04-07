using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200065B RID: 1627
	public class WeeklyRunScreenController : MonoBehaviour
	{
		// Token: 0x0600245D RID: 9309 RVA: 0x000AA9B7 File Offset: 0x000A8BB7
		private void OnEnable()
		{
			this.currentCycle = WeeklyRun.GetCurrentSeedCycle();
			this.UpdateLeaderboard();
		}

		// Token: 0x0600245E RID: 9310 RVA: 0x000AA9CA File Offset: 0x000A8BCA
		private void UpdateLeaderboard()
		{
			if (this.leaderboard)
			{
				this.leaderboard.SetRequestedInfo(WeeklyRun.GetLeaderboardName(1, this.currentCycle), this.leaderboard.currentRequestType, this.leaderboard.currentPage);
			}
		}

		// Token: 0x0600245F RID: 9311 RVA: 0x000AAA06 File Offset: 0x000A8C06
		public void SetCurrentLeaderboard(GameObject leaderboardGameObject)
		{
			this.leaderboard = leaderboardGameObject.GetComponent<LeaderboardController>();
			this.UpdateLeaderboard();
		}

		// Token: 0x06002460 RID: 9312 RVA: 0x000AAA1C File Offset: 0x000A8C1C
		private void Update()
		{
			uint currentSeedCycle = WeeklyRun.GetCurrentSeedCycle();
			if (currentSeedCycle != this.currentCycle)
			{
				this.currentCycle = currentSeedCycle;
				this.UpdateLeaderboard();
			}
			TimeSpan t = WeeklyRun.GetSeedCycleStartDateTime(this.currentCycle + 1u) - WeeklyRun.now;
			string @string = Language.GetString("WEEKLY_RUN_NEXT_CYCLE_COUNTDOWN_FORMAT");
			this.countdownLabel.text = string.Format(@string, t.Hours + t.Days * 24, t.Minutes, t.Seconds);
			if (t != this.lastCountdown)
			{
				this.lastCountdown = t;
				this.labelFadeValue = 0f;
			}
			this.labelFadeValue = Mathf.Max(this.labelFadeValue + Time.deltaTime * 1f, 0f);
			Color white = Color.white;
			if (t.Days == 0 && t.Hours == 0)
			{
				white.g = this.labelFadeValue;
				white.b = this.labelFadeValue;
			}
			this.countdownLabel.color = white;
		}

		// Token: 0x0400275C RID: 10076
		public LeaderboardController leaderboard;

		// Token: 0x0400275D RID: 10077
		public TextMeshProUGUI countdownLabel;

		// Token: 0x0400275E RID: 10078
		private uint currentCycle;

		// Token: 0x0400275F RID: 10079
		private TimeSpan lastCountdown;

		// Token: 0x04002760 RID: 10080
		private float labelFadeValue;
	}
}
