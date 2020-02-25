using System;

namespace RoR2.Achievements
{
	// Token: 0x02000691 RID: 1681
	[RegisterAchievement("ChargeTeleporterWhileNearDeath", "Items.WarCryOnMultiKill", null, null)]
	public class ChargeTeleporterWhileNearDeathAchievement : BaseAchievement
	{
		// Token: 0x06002764 RID: 10084 RVA: 0x000AA888 File Offset: 0x000A8A88
		public override void OnInstall()
		{
			base.OnInstall();
			TeleporterInteraction.onTeleporterChargedGlobal += this.OnTeleporterCharged;
		}

		// Token: 0x06002765 RID: 10085 RVA: 0x000AA8A1 File Offset: 0x000A8AA1
		public override void OnUninstall()
		{
			TeleporterInteraction.onTeleporterChargedGlobal -= this.OnTeleporterCharged;
			base.OnUninstall();
		}

		// Token: 0x06002766 RID: 10086 RVA: 0x000AA8BA File Offset: 0x000A8ABA
		private void OnTeleporterCharged(TeleporterInteraction teleporterInteraction)
		{
			this.Check();
		}

		// Token: 0x06002767 RID: 10087 RVA: 0x000AA8C4 File Offset: 0x000A8AC4
		private void Check()
		{
			if (base.localUser.cachedBody && base.localUser.cachedBody.healthComponent && base.localUser.cachedBody.healthComponent.alive && base.localUser.cachedBody.healthComponent.combinedHealthFraction <= 0.1f)
			{
				base.Grant();
			}
		}

		// Token: 0x040024ED RID: 9453
		private const float requirement = 0.1f;
	}
}
