using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006AE RID: 1710
	[RegisterAchievement("KillBossQuantityInRun", "Items.LunarSkillReplacements", null, null)]
	public class KillBossQuantityInRunAchievement : BaseAchievement
	{
		// Token: 0x060027EC RID: 10220 RVA: 0x000AB694 File Offset: 0x000A9894
		public override void OnInstall()
		{
			base.OnInstall();
			base.localUser.onMasterChanged += this.OnMasterChanged;
			base.userProfile.onStatsReceived += this.OnStatsReceived;
		}

		// Token: 0x060027ED RID: 10221 RVA: 0x000AB6CA File Offset: 0x000A98CA
		public override void OnUninstall()
		{
			base.userProfile.onStatsReceived -= this.OnStatsReceived;
			base.localUser.onMasterChanged -= this.OnMasterChanged;
			base.OnUninstall();
		}

		// Token: 0x060027EE RID: 10222 RVA: 0x000AB700 File Offset: 0x000A9900
		private void OnMasterChanged()
		{
			PlayerCharacterMasterController cachedMasterController = base.localUser.cachedMasterController;
			this.playerStatsComponent = ((cachedMasterController != null) ? cachedMasterController.GetComponent<PlayerStatsComponent>() : null);
		}

		// Token: 0x060027EF RID: 10223 RVA: 0x000AB71F File Offset: 0x000A991F
		private void OnStatsReceived()
		{
			this.Check();
		}

		// Token: 0x060027F0 RID: 10224 RVA: 0x000AB727 File Offset: 0x000A9927
		private void Check()
		{
			if (this.playerStatsComponent != null && KillBossQuantityInRunAchievement.requirement <= this.playerStatsComponent.currentStats.GetStatValueULong(StatDef.totalTeleporterBossKillsWitnessed))
			{
				base.Grant();
			}
		}

		// Token: 0x04002501 RID: 9473
		private static readonly ulong requirement = 15UL;

		// Token: 0x04002502 RID: 9474
		private PlayerStatsComponent playerStatsComponent;
	}
}
