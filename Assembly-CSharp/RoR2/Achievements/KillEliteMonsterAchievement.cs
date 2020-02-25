using System;
using UnityEngine;

namespace RoR2.Achievements
{
	// Token: 0x020006B3 RID: 1715
	[RegisterAchievement("KillEliteMonster", "Items.Medkit", null, typeof(KillEliteMonsterAchievement.KillEliteMonsterServerAchievement))]
	public class KillEliteMonsterAchievement : BaseAchievement
	{
		// Token: 0x060027FD RID: 10237 RVA: 0x000AA636 File Offset: 0x000A8836
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x060027FE RID: 10238 RVA: 0x000AA645 File Offset: 0x000A8845
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006B4 RID: 1716
		private class KillEliteMonsterServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002800 RID: 10240 RVA: 0x000AB7AA File Offset: 0x000A99AA
			public override void OnInstall()
			{
				base.OnInstall();
				GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeath;
			}

			// Token: 0x06002801 RID: 10241 RVA: 0x000AB7C3 File Offset: 0x000A99C3
			public override void OnUninstall()
			{
				GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeath;
				base.OnUninstall();
			}

			// Token: 0x06002802 RID: 10242 RVA: 0x000AB7DC File Offset: 0x000A99DC
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
