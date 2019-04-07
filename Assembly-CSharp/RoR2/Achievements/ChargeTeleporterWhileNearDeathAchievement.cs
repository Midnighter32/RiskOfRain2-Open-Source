using System;

namespace RoR2.Achievements
{
	// Token: 0x0200068B RID: 1675
	[RegisterAchievement("ChargeTeleporterWhileNearDeath", "Items.WarCryOnMultiKill", null, null)]
	public class ChargeTeleporterWhileNearDeathAchievement : BaseAchievement
	{
		// Token: 0x06002561 RID: 9569 RVA: 0x000AF094 File Offset: 0x000AD294
		public override void OnInstall()
		{
			base.OnInstall();
			TeleporterInteraction.onTeleporterChargedGlobal += this.OnTeleporterCharged;
		}

		// Token: 0x06002562 RID: 9570 RVA: 0x000AF0AD File Offset: 0x000AD2AD
		public override void OnUninstall()
		{
			TeleporterInteraction.onTeleporterChargedGlobal -= this.OnTeleporterCharged;
			base.OnUninstall();
		}

		// Token: 0x06002563 RID: 9571 RVA: 0x000AF0C6 File Offset: 0x000AD2C6
		private void OnTeleporterCharged(TeleporterInteraction teleporterInteraction)
		{
			this.Check();
		}

		// Token: 0x06002564 RID: 9572 RVA: 0x000AF0D0 File Offset: 0x000AD2D0
		private void Check()
		{
			if (this.localUser.cachedBody && this.localUser.cachedBody.healthComponent && this.localUser.cachedBody.healthComponent.alive && this.localUser.cachedBody.healthComponent.combinedHealthFraction <= 0.1f)
			{
				base.Grant();
			}
		}

		// Token: 0x04002861 RID: 10337
		private const float requirement = 0.1f;
	}
}
