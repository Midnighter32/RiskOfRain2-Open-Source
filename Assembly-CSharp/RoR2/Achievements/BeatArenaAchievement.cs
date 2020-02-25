using System;

namespace RoR2.Achievements
{
	// Token: 0x0200068D RID: 1677
	[RegisterAchievement("BeatArena", "Characters.Croco", null, typeof(BeatArenaAchievement.BeatArenaServerAchievement))]
	public class BeatArenaAchievement : BaseAchievement
	{
		// Token: 0x06002751 RID: 10065 RVA: 0x000AA636 File Offset: 0x000A8836
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x06002752 RID: 10066 RVA: 0x000AA645 File Offset: 0x000A8845
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x0200068E RID: 1678
		private class BeatArenaServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002754 RID: 10068 RVA: 0x000AA64D File Offset: 0x000A884D
			public override void OnInstall()
			{
				base.OnInstall();
				ArenaMissionController.onBeatArena += this.OnBeatArena;
			}

			// Token: 0x06002755 RID: 10069 RVA: 0x000AA666 File Offset: 0x000A8866
			public override void OnUninstall()
			{
				ArenaMissionController.onBeatArena -= this.OnBeatArena;
				base.OnInstall();
			}

			// Token: 0x06002756 RID: 10070 RVA: 0x000AA67F File Offset: 0x000A887F
			private void OnBeatArena()
			{
				base.Grant();
			}
		}
	}
}
