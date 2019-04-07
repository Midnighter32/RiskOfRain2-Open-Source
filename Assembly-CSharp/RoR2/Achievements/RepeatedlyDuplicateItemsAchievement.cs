using System;

namespace RoR2.Achievements
{
	// Token: 0x020006B4 RID: 1716
	[RegisterAchievement("RepeatedlyDuplicateItems", "Items.Firework", null, typeof(RepeatedlyDuplicateItemsAchievement.RepeatedlyDuplicateItemsServerAchievement))]
	public class RepeatedlyDuplicateItemsAchievement : BaseAchievement
	{
		// Token: 0x06002615 RID: 9749 RVA: 0x000AF2C0 File Offset: 0x000AD4C0
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x06002616 RID: 9750 RVA: 0x000AF2CF File Offset: 0x000AD4CF
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006B5 RID: 1717
		private class RepeatedlyDuplicateItemsServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002618 RID: 9752 RVA: 0x000B02D5 File Offset: 0x000AE4D5
			public override void OnInstall()
			{
				base.OnInstall();
				PurchaseInteraction.onItemSpentOnPurchase += this.OnItemSpentOnPurchase;
				Run.onRunStartGlobal += this.OnRunStartGlobal;
			}

			// Token: 0x06002619 RID: 9753 RVA: 0x000B02FF File Offset: 0x000AE4FF
			public override void OnUninstall()
			{
				base.OnInstall();
				PurchaseInteraction.onItemSpentOnPurchase -= this.OnItemSpentOnPurchase;
				Run.onRunStartGlobal -= this.OnRunStartGlobal;
			}

			// Token: 0x0600261A RID: 9754 RVA: 0x000B0329 File Offset: 0x000AE529
			private void OnRunStartGlobal(Run run)
			{
				this.progress = 0;
			}

			// Token: 0x0600261B RID: 9755 RVA: 0x000B0334 File Offset: 0x000AE534
			private void OnItemSpentOnPurchase(PurchaseInteraction purchaseInteraction, Interactor interactor)
			{
				CharacterBody currentBody = this.serverAchievementTracker.networkUser.GetCurrentBody();
				if (currentBody && currentBody.GetComponent<Interactor>() == interactor && purchaseInteraction.gameObject.name.Contains("Duplicator"))
				{
					ShopTerminalBehavior component = purchaseInteraction.GetComponent<ShopTerminalBehavior>();
					if (component)
					{
						ItemIndex itemIndex = component.CurrentPickupIndex().itemIndex;
						if (this.trackingItemIndex != itemIndex)
						{
							this.trackingItemIndex = itemIndex;
							this.progress = 0;
						}
						this.progress++;
						if (this.progress >= 7)
						{
							base.Grant();
						}
					}
				}
			}

			// Token: 0x04002879 RID: 10361
			private const int requirement = 7;

			// Token: 0x0400287A RID: 10362
			private ItemIndex trackingItemIndex = ItemIndex.None;

			// Token: 0x0400287B RID: 10363
			private int progress;
		}
	}
}
