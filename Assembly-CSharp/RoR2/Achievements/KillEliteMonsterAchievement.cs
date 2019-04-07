using System;
using UnityEngine;

namespace RoR2.Achievements
{
	// Token: 0x020006AA RID: 1706
	[RegisterAchievement("KillEliteMonster", "Items.Medkit", null, typeof(KillEliteMonsterAchievement.KillEliteMonsterServerAchievement))]
	public class KillEliteMonsterAchievement : BaseAchievement
	{
		// Token: 0x060025EB RID: 9707 RVA: 0x000AF2C0 File Offset: 0x000AD4C0
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x060025EC RID: 9708 RVA: 0x000AF2CF File Offset: 0x000AD4CF
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006AB RID: 1707
		private class KillEliteMonsterServerAchievement : BaseServerAchievement
		{
			// Token: 0x060025EE RID: 9710 RVA: 0x000AFE8F File Offset: 0x000AE08F
			public override void OnInstall()
			{
				base.OnInstall();
				GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeath;
			}

			// Token: 0x060025EF RID: 9711 RVA: 0x000AFEA8 File Offset: 0x000AE0A8
			public override void OnUninstall()
			{
				GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeath;
				base.OnUninstall();
			}

			// Token: 0x060025F0 RID: 9712 RVA: 0x000AFEC4 File Offset: 0x000AE0C4
			private void OnCharacterDeath(DamageReport damageReport)
			{
				if (!damageReport.victim)
				{
					return;
				}
				CharacterBody body = damageReport.victim.body;
				if (!body || !body.isElite)
				{
					return;
				}
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
				if (component.masterObject == this.serverAchievementTracker.networkUser.masterObject)
				{
					base.Grant();
				}
			}
		}
	}
}
