using System;

namespace RoR2.Achievements.Mage
{
	// Token: 0x020006E4 RID: 1764
	[RegisterAchievement("MageMultiKill", "Skills.Mage.LightningBolt", "FreeMage", typeof(MageMultiKillAchievement.MageMultiKillServerAchievement))]
	public class MageMultiKillAchievement : BaseAchievement
	{
		// Token: 0x060028FE RID: 10494 RVA: 0x000AD129 File Offset: 0x000AB329
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("MageBody");
		}

		// Token: 0x060028FF RID: 10495 RVA: 0x000AC8DE File Offset: 0x000AAADE
		protected override void OnBodyRequirementMet()
		{
			base.OnBodyRequirementMet();
			base.SetServerTracked(true);
		}

		// Token: 0x06002900 RID: 10496 RVA: 0x000AC8ED File Offset: 0x000AAAED
		protected override void OnBodyRequirementBroken()
		{
			base.SetServerTracked(false);
			base.OnBodyRequirementBroken();
		}

		// Token: 0x0400253D RID: 9533
		private static readonly int requirement = 20;

		// Token: 0x020006E5 RID: 1765
		private class MageMultiKillServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002903 RID: 10499 RVA: 0x000AD4B1 File Offset: 0x000AB6B1
			public override void OnInstall()
			{
				base.OnInstall();
				RoR2Application.onFixedUpdate += this.OnFixedUpdate;
			}

			// Token: 0x06002904 RID: 10500 RVA: 0x000AD4CA File Offset: 0x000AB6CA
			public override void OnUninstall()
			{
				RoR2Application.onFixedUpdate -= this.OnFixedUpdate;
				base.OnUninstall();
			}

			// Token: 0x06002905 RID: 10501 RVA: 0x000AD4E4 File Offset: 0x000AB6E4
			private void OnFixedUpdate()
			{
				CharacterBody currentBody = base.GetCurrentBody();
				if (currentBody && MageMultiKillAchievement.requirement <= currentBody.multiKillCount)
				{
					base.Grant();
				}
			}
		}
	}
}
