using System;

namespace RoR2.Achievements
{
	// Token: 0x020006BE RID: 1726
	[RegisterAchievement("MaxHealingShrine", "Items.PassiveHealing", null, typeof(MaxHealingShrineAchievement.MaxHealingShrineServerAchievement))]
	public class MaxHealingShrineAchievement : BaseAchievement
	{
		// Token: 0x0600282E RID: 10286 RVA: 0x000AA636 File Offset: 0x000A8836
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x0600282F RID: 10287 RVA: 0x000AA645 File Offset: 0x000A8845
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006BF RID: 1727
		private class MaxHealingShrineServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002831 RID: 10289 RVA: 0x000ABCC7 File Offset: 0x000A9EC7
			public override void OnInstall()
			{
				base.OnInstall();
				ShrineHealingBehavior.onActivated += this.OnHealingShrineActivated;
			}

			// Token: 0x06002832 RID: 10290 RVA: 0x000ABCE0 File Offset: 0x000A9EE0
			public override void OnUninstall()
			{
				ShrineHealingBehavior.onActivated -= this.OnHealingShrineActivated;
				base.OnUninstall();
			}

			// Token: 0x06002833 RID: 10291 RVA: 0x000ABCFC File Offset: 0x000A9EFC
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

			// Token: 0x0400250A RID: 9482
			private const int requirement = 2;
		}
	}
}
