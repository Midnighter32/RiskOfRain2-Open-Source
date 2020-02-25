using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000650 RID: 1616
	public class WeeklyRunScreenController : MonoBehaviour
	{
		// Token: 0x06002601 RID: 9729 RVA: 0x000A54F3 File Offset: 0x000A36F3
		private void OnEnable()
		{
			this.currentCycle = WeeklyRun.GetCurrentSeedCycle();
			this.UpdateLeaderboard();
		}

		// Token: 0x06002602 RID: 9730 RVA: 0x000A5506 File Offset: 0x000A3706
		private void UpdateLeaderboard()
		{
			if (this.leaderboard)
			{
				this.leaderboard.SetRequestedInfo(WeeklyRun.GetLeaderboardName(1, this.currentCycle), this.leaderboard.currentRequestType, this.leaderboard.currentPage);
			}
		}

		// Token: 0x06002603 RID: 9731 RVA: 0x000A5542 File Offset: 0x000A3742
		public void SetCurrentLeaderboard(GameObject leaderboardGameObject)
		{
			this.leaderboard = leaderboardGameObject.GetComponent<LeaderboardController>();
			this.UpdateLeaderboard();
		}

		// Token: 0x06002604 RID: 9732 RVA: 0x000A5558 File Offset: 0x000A3758
		private void Update()
		{
			uint currentSeedCycle = WeeklyRun.GetCurrentSeedCycle();
			if (currentSeedCycle != this.currentCycle)
			{
				this.currentCycle = currentSeedCycle;
				this.UpdateLeaderboard();
			}
			TimeSpan t = WeeklyRun.GetSeedCycleStartDateTime(this.currentCycle + 1U) - WeeklyRun.now;
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

		// Token: 0x040023C3 RID: 9155
		public LeaderboardController leaderboard;

		// Token: 0x040023C4 RID: 9156
		public TextMeshProUGUI countdownLabel;

		// Token: 0x040023C5 RID: 9157
		private uint currentCycle;

		// Token: 0x040023C6 RID: 9158
		private TimeSpan lastCountdown;

		// Token: 0x040023C7 RID: 9159
		private float labelFadeValue;
	}
}
