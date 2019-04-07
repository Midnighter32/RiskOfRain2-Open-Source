using System;

namespace RoR2.Achievements
{
	// Token: 0x02000692 RID: 1682
	[RegisterAchievement("CompleteTeleporter", "Items.BossDamageBonus", null, null)]
	public class CompleteTeleporterAchievement : BaseAchievement
	{
		// Token: 0x06002580 RID: 9600 RVA: 0x000AF3D3 File Offset: 0x000AD5D3
		public override void OnInstall()
		{
			base.OnInstall();
			TeleporterInteraction.onTeleporterFinishGlobal += this.OnTeleporterFinish;
		}

		// Token: 0x06002581 RID: 9601 RVA: 0x000AF3EC File Offset: 0x000AD5EC
		public override void OnUninstall()
		{
			TeleporterInteraction.onTeleporterFinishGlobal -= this.OnTeleporterFinish;
			base.OnUninstall();
		}

		// Token: 0x06002582 RID: 9602 RVA: 0x000AF405 File Offset: 0x000AD605
		private void OnTeleporterFinish(TeleporterInteraction teleporterInteraction)
		{
			this.Check();
		}

		// Token: 0x06002583 RID: 9603 RVA: 0x000AF410 File Offset: 0x000AD610
		private void Check()
		{
			if (this.localUser.cachedBody && this.localUser.cachedBody.healthComponent && this.localUser.cachedBody.healthComponent.alive)
			{
				base.Grant();
			}
		}
	}
}
