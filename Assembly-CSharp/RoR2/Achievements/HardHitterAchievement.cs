using System;

namespace RoR2.Achievements
{
	// Token: 0x020006AD RID: 1709
	[RegisterAchievement("HardHitter", "Items.ShockNearby", null, null)]
	public class HardHitterAchievement : BaseAchievement
	{
		// Token: 0x060027E8 RID: 10216 RVA: 0x000AB628 File Offset: 0x000A9828
		public override void OnInstall()
		{
			base.OnInstall();
			GlobalEventManager.onClientDamageNotified += this.CheckDamage;
		}

		// Token: 0x060027E9 RID: 10217 RVA: 0x000AB641 File Offset: 0x000A9841
		public override void OnUninstall()
		{
			GlobalEventManager.onClientDamageNotified -= this.CheckDamage;
			base.OnUninstall();
		}

		// Token: 0x060027EA RID: 10218 RVA: 0x000AB65A File Offset: 0x000A985A
		public void CheckDamage(DamageDealtMessage damageDealtMessage)
		{
			if (damageDealtMessage.damage >= 5000f && damageDealtMessage.attacker && damageDealtMessage.attacker == base.localUser.cachedBodyObject)
			{
				base.Grant();
			}
		}

		// Token: 0x04002500 RID: 9472
		private const float requirement = 5000f;
	}
}
