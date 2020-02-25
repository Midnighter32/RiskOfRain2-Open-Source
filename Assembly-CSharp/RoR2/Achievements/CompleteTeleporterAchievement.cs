using System;

namespace RoR2.Achievements
{
	// Token: 0x02000698 RID: 1688
	[RegisterAchievement("CompleteTeleporter", "Items.BossDamageBonus", null, null)]
	public class CompleteTeleporterAchievement : BaseAchievement
	{
		// Token: 0x06002783 RID: 10115 RVA: 0x000AABAC File Offset: 0x000A8DAC
		public override void OnInstall()
		{
			base.OnInstall();
			TeleporterInteraction.onTeleporterFinishGlobal += this.OnTeleporterFinish;
		}

		// Token: 0x06002784 RID: 10116 RVA: 0x000AABC5 File Offset: 0x000A8DC5
		public override void OnUninstall()
		{
			TeleporterInteraction.onTeleporterFinishGlobal -= this.OnTeleporterFinish;
			base.OnUninstall();
		}

		// Token: 0x06002785 RID: 10117 RVA: 0x000AABDE File Offset: 0x000A8DDE
		private void OnTeleporterFinish(TeleporterInteraction teleporterInteraction)
		{
			this.Check();
		}

		// Token: 0x06002786 RID: 10118 RVA: 0x000AABE8 File Offset: 0x000A8DE8
		private void Check()
		{
			if (base.localUser.cachedBody && base.localUser.cachedBody.healthComponent && base.localUser.cachedBody.healthComponent.alive)
			{
				base.Grant();
			}
		}
	}
}
