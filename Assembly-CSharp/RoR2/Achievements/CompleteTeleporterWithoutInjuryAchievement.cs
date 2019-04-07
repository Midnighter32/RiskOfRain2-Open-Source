using System;

namespace RoR2.Achievements
{
	// Token: 0x02000693 RID: 1683
	[RegisterAchievement("CompleteTeleporterWithoutInjury", "Items.SecondarySkillMagazine", null, null)]
	public class CompleteTeleporterWithoutInjuryAchievement : BaseAchievement
	{
		// Token: 0x06002585 RID: 9605 RVA: 0x000AF463 File Offset: 0x000AD663
		public override void OnInstall()
		{
			base.OnInstall();
			TeleporterInteraction.onTeleporterBeginChargingGlobal += this.OnTeleporterBeginCharging;
			TeleporterInteraction.onTeleporterChargedGlobal += this.OnTeleporterCharged;
			GlobalEventManager.onClientDamageNotified += this.OnClientDamageNotified;
		}

		// Token: 0x06002586 RID: 9606 RVA: 0x000AF49E File Offset: 0x000AD69E
		public override void OnUninstall()
		{
			TeleporterInteraction.onTeleporterBeginChargingGlobal -= this.OnTeleporterBeginCharging;
			TeleporterInteraction.onTeleporterChargedGlobal -= this.OnTeleporterCharged;
			GlobalEventManager.onClientDamageNotified -= this.OnClientDamageNotified;
			base.OnUninstall();
		}

		// Token: 0x06002587 RID: 9607 RVA: 0x000AF4D9 File Offset: 0x000AD6D9
		private void OnTeleporterBeginCharging(TeleporterInteraction teleporterInteraction)
		{
			this.hasBeenHit = false;
		}

		// Token: 0x06002588 RID: 9608 RVA: 0x000AF4E2 File Offset: 0x000AD6E2
		private void OnTeleporterCharged(TeleporterInteraction teleporterInteraction)
		{
			this.Check();
		}

		// Token: 0x06002589 RID: 9609 RVA: 0x000AF4EA File Offset: 0x000AD6EA
		private void OnClientDamageNotified(DamageDealtMessage damageDealtMessage)
		{
			if (!this.hasBeenHit && damageDealtMessage.victim && damageDealtMessage.victim == this.localUser.cachedBodyObject)
			{
				this.hasBeenHit = true;
			}
		}

		// Token: 0x0600258A RID: 9610 RVA: 0x000AF520 File Offset: 0x000AD720
		private void Check()
		{
			if (this.localUser.cachedBody && this.localUser.cachedBody.healthComponent && this.localUser.cachedBody.healthComponent.alive && !this.hasBeenHit)
			{
				base.Grant();
			}
		}

		// Token: 0x04002868 RID: 10344
		private bool hasBeenHit;
	}
}
