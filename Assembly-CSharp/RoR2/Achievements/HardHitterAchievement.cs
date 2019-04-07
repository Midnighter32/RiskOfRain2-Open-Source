using System;

namespace RoR2.Achievements
{
	// Token: 0x020006A5 RID: 1701
	[RegisterAchievement("HardHitter", "Items.ShockNearby", null, null)]
	public class HardHitterAchievement : BaseAchievement
	{
		// Token: 0x060025DD RID: 9693 RVA: 0x000AFDDC File Offset: 0x000ADFDC
		public override void OnInstall()
		{
			base.OnInstall();
			GlobalEventManager.onClientDamageNotified += this.CheckDamage;
		}

		// Token: 0x060025DE RID: 9694 RVA: 0x000AFDF5 File Offset: 0x000ADFF5
		public override void OnUninstall()
		{
			GlobalEventManager.onClientDamageNotified -= this.CheckDamage;
			base.OnUninstall();
		}

		// Token: 0x060025DF RID: 9695 RVA: 0x000AFE0E File Offset: 0x000AE00E
		public void CheckDamage(DamageDealtMessage damageDealtMessage)
		{
			if (damageDealtMessage.damage >= 5000f && damageDealtMessage.attacker && damageDealtMessage.attacker == this.localUser.cachedBodyObject)
			{
				base.Grant();
			}
		}

		// Token: 0x04002872 RID: 10354
		private const float requirement = 5000f;
	}
}
