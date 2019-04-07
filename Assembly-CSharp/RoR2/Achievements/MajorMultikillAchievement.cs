using System;
using UnityEngine;

namespace RoR2.Achievements
{
	// Token: 0x020006AF RID: 1711
	[RegisterAchievement("MajorMultikill", "Items.BurnNearby", null, typeof(MajorMultikillAchievement.MajorMultikillServerAchievement))]
	public class MajorMultikillAchievement : BaseAchievement
	{
		// Token: 0x06002603 RID: 9731 RVA: 0x000AF2C0 File Offset: 0x000AD4C0
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x06002604 RID: 9732 RVA: 0x000AF2CF File Offset: 0x000AD4CF
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006B0 RID: 1712
		private class MajorMultikillServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002606 RID: 9734 RVA: 0x000B014A File Offset: 0x000AE34A
			public override void OnInstall()
			{
				base.OnInstall();
				GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeath;
			}

			// Token: 0x06002607 RID: 9735 RVA: 0x000B0163 File Offset: 0x000AE363
			public override void OnUninstall()
			{
				GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeath;
				base.OnUninstall();
			}

			// Token: 0x06002608 RID: 9736 RVA: 0x000B017C File Offset: 0x000AE37C
			private void OnCharacterDeath(DamageReport damageReport)
			{
				GameObject attacker = damageReport.damageInfo.attacker;
				if (!attacker)
				{
					return;
				}
				CharacterBody component = attacker.GetComponent<CharacterBody>();
				if (!component)
				{
					return;
				}
				if (component.multiKillCount >= 15 && component.masterObject == this.serverAchievementTracker.networkUser.masterObject)
				{
					base.Grant();
				}
			}

			// Token: 0x04002876 RID: 10358
			private const int multiKillThreshold = 15;
		}
	}
}
