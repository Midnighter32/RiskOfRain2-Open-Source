using System;

namespace RoR2.Achievements
{
	// Token: 0x02000699 RID: 1689
	[RegisterAchievement("Discover10UniqueTier1", "Items.Crowbar", null, null)]
	public class Discover10UniqueTier1Achievement : BaseAchievement
	{
		// Token: 0x060025A2 RID: 9634 RVA: 0x000AF7FF File Offset: 0x000AD9FF
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onPickupDiscovered += this.OnPickupDiscovered;
			this.Check();
		}

		// Token: 0x060025A3 RID: 9635 RVA: 0x000AF824 File Offset: 0x000ADA24
		public override void OnUninstall()
		{
			this.userProfile.onPickupDiscovered -= this.OnPickupDiscovered;
			base.OnUninstall();
		}

		// Token: 0x060025A4 RID: 9636 RVA: 0x000AF843 File Offset: 0x000ADA43
		public override float ProgressForAchievement()
		{
			return (float)this.UniqueTier1Discovered() / 10f;
		}

		// Token: 0x060025A5 RID: 9637 RVA: 0x000AF854 File Offset: 0x000ADA54
		private void OnPickupDiscovered(PickupIndex pickupIndex)
		{
			ItemIndex itemIndex = pickupIndex.itemIndex;
			if (itemIndex != ItemIndex.None && ItemCatalog.GetItemDef(itemIndex).tier == ItemTier.Tier1)
			{
				this.Check();
			}
		}

		// Token: 0x060025A6 RID: 9638 RVA: 0x000AF880 File Offset: 0x000ADA80
		private int UniqueTier1Discovered()
		{
			int num = 0;
			for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < ItemIndex.Count; itemIndex++)
			{
				if (ItemCatalog.GetItemDef(itemIndex).tier == ItemTier.Tier1 && this.userProfile.HasDiscoveredPickup(new PickupIndex(itemIndex)))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x060025A7 RID: 9639 RVA: 0x000AF8C1 File Offset: 0x000ADAC1
		private void Check()
		{
			if (this.UniqueTier1Discovered() >= 10)
			{
				base.Grant();
			}
		}

		// Token: 0x0400286C RID: 10348
		private const int requirement = 10;
	}
}
