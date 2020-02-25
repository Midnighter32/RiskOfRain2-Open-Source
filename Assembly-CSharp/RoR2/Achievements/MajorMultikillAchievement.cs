using System;
using UnityEngine;

namespace RoR2.Achievements
{
	// Token: 0x020006BC RID: 1724
	[RegisterAchievement("MajorMultikill", "Items.BurnNearby", null, typeof(MajorMultikillAchievement.MajorMultikillServerAchievement))]
	public class MajorMultikillAchievement : BaseAchievement
	{
		// Token: 0x06002827 RID: 10279 RVA: 0x000AA636 File Offset: 0x000A8836
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x06002828 RID: 10280 RVA: 0x000AA645 File Offset: 0x000A8845
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006BD RID: 1725
		private class MajorMultikillServerAchievement : BaseServerAchievement
		{
			// Token: 0x0600282A RID: 10282 RVA: 0x000ABC36 File Offset: 0x000A9E36
			public override void OnInstall()
			{
				base.OnInstall();
				GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeath;
			}

			// Token: 0x0600282B RID: 10283 RVA: 0x000ABC4F File Offset: 0x000A9E4F
			public override void OnUninstall()
			{
				GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeath;
				base.OnUninstall();
			}

			// Token: 0x0600282C RID: 10284 RVA: 0x000ABC68 File Offset: 0x000A9E68
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

			// Token: 0x04002509 RID: 9481
			private const int multiKillThreshold = 15;
		}
	}
}
