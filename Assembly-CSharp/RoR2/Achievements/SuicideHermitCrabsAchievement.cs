using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006B9 RID: 1721
	[RegisterAchievement("SuicideHermitCrabs", "Items.AutoCastEquipment", null, typeof(SuicideHermitCrabsAchievement.SuicideHermitCrabsServerAchievement))]
	public class SuicideHermitCrabsAchievement : BaseAchievement
	{
		// Token: 0x0600262B RID: 9771 RVA: 0x000B0599 File Offset: 0x000AE799
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			base.SetServerTracked(true);
		}

		// Token: 0x0600262C RID: 9772 RVA: 0x000B05BF File Offset: 0x000AE7BF
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x0600262D RID: 9773 RVA: 0x000B05DE File Offset: 0x000AE7DE
		private void Check()
		{
			if (this.userProfile.statSheet.GetStatValueULong(StatDef.suicideHermitCrabsAchievementProgress) >= 20UL)
			{
				base.Grant();
			}
		}

		// Token: 0x0600262E RID: 9774 RVA: 0x000B0600 File Offset: 0x000AE800
		public override float ProgressForAchievement()
		{
			return this.userProfile.statSheet.GetStatValueULong(StatDef.suicideHermitCrabsAchievementProgress) / 20f;
		}

		// Token: 0x0400287E RID: 10366
		private const int requirement = 20;

		// Token: 0x020006BA RID: 1722
		private class SuicideHermitCrabsServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002630 RID: 9776 RVA: 0x000B0620 File Offset: 0x000AE820
			public override void OnInstall()
			{
				base.OnInstall();
				GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeath;
			}

			// Token: 0x06002631 RID: 9777 RVA: 0x000B0639 File Offset: 0x000AE839
			public override void OnUninstall()
			{
				GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeath;
				base.OnUninstall();
			}

			// Token: 0x06002632 RID: 9778 RVA: 0x000B0654 File Offset: 0x000AE854
			private void OnCharacterDeath(DamageReport damageReport)
			{
				if (!damageReport.victimBody)
				{
					return;
				}
				if (damageReport.damageInfo.attacker)
				{
					return;
				}
				if (damageReport.victim.name.Contains("HermitCrab") && damageReport.victimBody.teamComponent.teamIndex != TeamIndex.Player)
				{
					PlayerStatsComponent masterPlayerStatsComponent = base.networkUser.masterPlayerStatsComponent;
					if (masterPlayerStatsComponent)
					{
						masterPlayerStatsComponent.currentStats.PushStatValue(StatDef.suicideHermitCrabsAchievementProgress, 1UL);
					}
				}
			}
		}
	}
}
