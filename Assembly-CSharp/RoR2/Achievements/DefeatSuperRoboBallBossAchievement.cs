using System;

namespace RoR2.Achievements
{
	// Token: 0x0200069E RID: 1694
	[RegisterAchievement("DefeatSuperRoboBallBoss", "Characters.Loader", null, typeof(DefeatSuperRoboBallBossAchievement.DefeatSuperRoboBallBossServerAchievement))]
	public class DefeatSuperRoboBallBossAchievement : BaseAchievement
	{
		// Token: 0x060027A0 RID: 10144 RVA: 0x000AA636 File Offset: 0x000A8836
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x060027A1 RID: 10145 RVA: 0x000AA645 File Offset: 0x000A8845
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x0200069F RID: 1695
		private class DefeatSuperRoboBallBossServerAchievement : BaseServerAchievement
		{
			// Token: 0x060027A3 RID: 10147 RVA: 0x000AAF53 File Offset: 0x000A9153
			public override void OnInstall()
			{
				base.OnInstall();
				this.superRoboBallBossBodyIndex = BodyCatalog.FindBodyIndex("SuperRoboBallBossBody");
				GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeathGlobal;
			}

			// Token: 0x060027A4 RID: 10148 RVA: 0x000AAF7C File Offset: 0x000A917C
			private void OnCharacterDeathGlobal(DamageReport damageReport)
			{
				if (damageReport.victimBodyIndex == this.superRoboBallBossBodyIndex && damageReport.victimTeamIndex != TeamIndex.Player)
				{
					base.Grant();
				}
			}

			// Token: 0x060027A5 RID: 10149 RVA: 0x000AAF9B File Offset: 0x000A919B
			public override void OnUninstall()
			{
				GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeathGlobal;
				base.OnUninstall();
			}

			// Token: 0x040024F7 RID: 9463
			private int superRoboBallBossBodyIndex;

			// Token: 0x040024F8 RID: 9464
			private static readonly ulong requirement = 1UL;
		}
	}
}
