using System;

namespace RoR2.Achievements.Commando
{
	// Token: 0x020006F1 RID: 1777
	[RegisterAchievement("CommandoKillOverloadingWorm", "Skills.Commando.FireShotgunBlast", null, typeof(CommandoKillOverloadingWormAchievement.CommandoKillOverloadingWormServerAchievement))]
	public class CommandoKillOverloadingWormAchievement : BaseAchievement
	{
		// Token: 0x06002947 RID: 10567 RVA: 0x000ADAE6 File Offset: 0x000ABCE6
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("CommandoBody");
		}

		// Token: 0x06002948 RID: 10568 RVA: 0x000AC8DE File Offset: 0x000AAADE
		protected override void OnBodyRequirementMet()
		{
			base.OnBodyRequirementMet();
			base.SetServerTracked(true);
		}

		// Token: 0x06002949 RID: 10569 RVA: 0x000AC8ED File Offset: 0x000AAAED
		protected override void OnBodyRequirementBroken()
		{
			base.SetServerTracked(false);
			base.OnBodyRequirementBroken();
		}

		// Token: 0x0400254E RID: 9550
		private int commandoBodyIndex;

		// Token: 0x020006F2 RID: 1778
		public class CommandoKillOverloadingWormServerAchievement : BaseServerAchievement
		{
			// Token: 0x0600294B RID: 10571 RVA: 0x000ADAF2 File Offset: 0x000ABCF2
			public override void OnInstall()
			{
				base.OnInstall();
				this.overloadingWormBodyIndex = BodyCatalog.FindBodyIndex("ElectricWormBody");
				GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeath;
			}

			// Token: 0x0600294C RID: 10572 RVA: 0x000ADB1B File Offset: 0x000ABD1B
			public override void OnUninstall()
			{
				GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeath;
				base.OnUninstall();
			}

			// Token: 0x0600294D RID: 10573 RVA: 0x000ADB34 File Offset: 0x000ABD34
			private void OnCharacterDeath(DamageReport damageReport)
			{
				if (damageReport.victimBody && damageReport.victimBody.bodyIndex == this.overloadingWormBodyIndex && base.IsCurrentBody(damageReport.damageInfo.attacker))
				{
					base.Grant();
				}
			}

			// Token: 0x0400254F RID: 9551
			private int overloadingWormBodyIndex;
		}
	}
}
