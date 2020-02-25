using System;

namespace RoR2.Achievements
{
	// Token: 0x02000699 RID: 1689
	[RegisterAchievement("CompleteTeleporterWithoutInjury", "Items.SecondarySkillMagazine", null, null)]
	public class CompleteTeleporterWithoutInjuryAchievement : BaseAchievement
	{
		// Token: 0x06002788 RID: 10120 RVA: 0x000AAC3B File Offset: 0x000A8E3B
		public override void OnInstall()
		{
			base.OnInstall();
			TeleporterInteraction.onTeleporterBeginChargingGlobal += this.OnTeleporterBeginCharging;
			TeleporterInteraction.onTeleporterChargedGlobal += this.OnTeleporterCharged;
			GlobalEventManager.onClientDamageNotified += this.OnClientDamageNotified;
		}

		// Token: 0x06002789 RID: 10121 RVA: 0x000AAC76 File Offset: 0x000A8E76
		public override void OnUninstall()
		{
			TeleporterInteraction.onTeleporterBeginChargingGlobal -= this.OnTeleporterBeginCharging;
			TeleporterInteraction.onTeleporterChargedGlobal -= this.OnTeleporterCharged;
			GlobalEventManager.onClientDamageNotified -= this.OnClientDamageNotified;
			base.OnUninstall();
		}

		// Token: 0x0600278A RID: 10122 RVA: 0x000AACB1 File Offset: 0x000A8EB1
		private void OnTeleporterBeginCharging(TeleporterInteraction teleporterInteraction)
		{
			this.hasBeenHit = false;
		}

		// Token: 0x0600278B RID: 10123 RVA: 0x000AACBA File Offset: 0x000A8EBA
		private void OnTeleporterCharged(TeleporterInteraction teleporterInteraction)
		{
			this.Check();
		}

		// Token: 0x0600278C RID: 10124 RVA: 0x000AACC2 File Offset: 0x000A8EC2
		private void OnClientDamageNotified(DamageDealtMessage damageDealtMessage)
		{
			if (!this.hasBeenHit && damageDealtMessage.victim && damageDealtMessage.victim == base.localUser.cachedBodyObject)
			{
				this.hasBeenHit = true;
			}
		}

		// Token: 0x0600278D RID: 10125 RVA: 0x000AACF8 File Offset: 0x000A8EF8
		private void Check()
		{
			if (base.localUser.cachedBody && base.localUser.cachedBody.healthComponent && base.localUser.cachedBody.healthComponent.alive && !this.hasBeenHit)
			{
				base.Grant();
			}
		}

		// Token: 0x040024F4 RID: 9460
		private bool hasBeenHit;
	}
}
