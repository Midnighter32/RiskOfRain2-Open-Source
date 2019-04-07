using System;

namespace RoR2.Achievements
{
	// Token: 0x020006B1 RID: 1713
	[RegisterAchievement("MaxHealingShrine", "Items.PassiveHealing", null, typeof(MaxHealingShrineAchievement.MaxHealingShrineServerAchievement))]
	public class MaxHealingShrineAchievement : BaseAchievement
	{
		// Token: 0x0600260A RID: 9738 RVA: 0x000AF2C0 File Offset: 0x000AD4C0
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x0600260B RID: 9739 RVA: 0x000AF2CF File Offset: 0x000AD4CF
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006B2 RID: 1714
		private class MaxHealingShrineServerAchievement : BaseServerAchievement
		{
			// Token: 0x0600260D RID: 9741 RVA: 0x000B01DB File Offset: 0x000AE3DB
			public override void OnInstall()
			{
				base.OnInstall();
				ShrineHealingBehavior.onActivated += this.OnHealingShrineActivated;
			}

			// Token: 0x0600260E RID: 9742 RVA: 0x000B01F4 File Offset: 0x000AE3F4
			public override void OnUninstall()
			{
				ShrineHealingBehavior.onActivated -= this.OnHealingShrineActivated;
				base.OnUninstall();
			}

			// Token: 0x0600260F RID: 9743 RVA: 0x000B0210 File Offset: 0x000AE410
			private void OnHealingShrineActivated(ShrineHealingBehavior shrine, Interactor activator)
			{
				if (shrine.purchaseCount >= shrine.maxPurchaseCount)
				{
					CharacterBody currentBody = base.GetCurrentBody();
					if (currentBody && currentBody.gameObject == activator.gameObject)
					{
						base.Grant();
					}
				}
			}

			// Token: 0x04002877 RID: 10359
			private const int requirement = 2;
		}
	}
}
