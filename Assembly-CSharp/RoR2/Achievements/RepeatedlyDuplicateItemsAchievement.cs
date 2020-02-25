using System;

namespace RoR2.Achievements
{
	// Token: 0x020006C3 RID: 1731
	[RegisterAchievement("RepeatedlyDuplicateItems", "Items.Firework", null, typeof(RepeatedlyDuplicateItemsAchievement.RepeatedlyDuplicateItemsServerAchievement))]
	public class RepeatedlyDuplicateItemsAchievement : BaseAchievement
	{
		// Token: 0x06002843 RID: 10307 RVA: 0x000AA636 File Offset: 0x000A8836
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x06002844 RID: 10308 RVA: 0x000AA645 File Offset: 0x000A8845
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006C4 RID: 1732
		private class RepeatedlyDuplicateItemsServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002846 RID: 10310 RVA: 0x000ABE59 File Offset: 0x000AA059
			public override void OnInstall()
			{
				base.OnInstall();
				PurchaseInteraction.onItemSpentOnPurchase += this.OnItemSpentOnPurchase;
				Run.onRunStartGlobal += this.OnRunStartGlobal;
			}

			// Token: 0x06002847 RID: 10311 RVA: 0x000ABE83 File Offset: 0x000AA083
			public override void OnUninstall()
			{
				base.OnInstall();
				PurchaseInteraction.onItemSpentOnPurchase -= this.OnItemSpentOnPurchase;
				Run.onRunStartGlobal -= this.OnRunStartGlobal;
			}

			// Token: 0x06002848 RID: 10312 RVA: 0x000ABEAD File Offset: 0x000AA0AD
			private void OnRunStartGlobal(Run run)
			{
				this.progress = 0;
			}

			// Token: 0x06002849 RID: 10313 RVA: 0x000ABEB8 File Offset: 0x000AA0B8
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

			// Token: 0x0400250E RID: 9486
			private const int requirement = 7;

			// Token: 0x0400250F RID: 9487
			private ItemIndex trackingItemIndex = ItemIndex.None;

			// Token: 0x04002510 RID: 9488
			private int progress;
		}
	}
}
