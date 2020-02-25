using System;

namespace RoR2.Achievements
{
	// Token: 0x020006A1 RID: 1697
	[RegisterAchievement("Discover10UniqueTier1", "Items.Crowbar", null, null)]
	public class Discover10UniqueTier1Achievement : BaseAchievement
	{
		// Token: 0x060027AD RID: 10157 RVA: 0x000AB041 File Offset: 0x000A9241
		public override void OnInstall()
		{
			base.OnInstall();
			base.userProfile.onPickupDiscovered += this.OnPickupDiscovered;
			this.Check();
		}

		// Token: 0x060027AE RID: 10158 RVA: 0x000AB066 File Offset: 0x000A9266
		public override void OnUninstall()
		{
			base.userProfile.onPickupDiscovered -= this.OnPickupDiscovered;
			base.OnUninstall();
		}

		// Token: 0x060027AF RID: 10159 RVA: 0x000AB085 File Offset: 0x000A9285
		public override float ProgressForAchievement()
		{
			return (float)this.UniqueTier1Discovered() / 10f;
		}

		// Token: 0x060027B0 RID: 10160 RVA: 0x000AB094 File Offset: 0x000A9294
		private void OnPickupDiscovered(PickupIndex pickupIndex)
		{
			ItemIndex itemIndex = pickupIndex.itemIndex;
			if (itemIndex != ItemIndex.None && ItemCatalog.GetItemDef(itemIndex).tier == ItemTier.Tier1)
			{
				this.Check();
			}
		}

		// Token: 0x060027B1 RID: 10161 RVA: 0x000AB0C0 File Offset: 0x000A92C0
		private int UniqueTier1Discovered()
		{
			int num = 0;
			ItemIndex itemIndex = ItemIndex.Syringe;
			ItemIndex itemCount = (ItemIndex)ItemCatalog.itemCount;
			while (itemIndex < itemCount)
			{
				if (ItemCatalog.GetItemDef(itemIndex).tier == ItemTier.Tier1 && base.userProfile.HasDiscoveredPickup(new PickupIndex(itemIndex)))
				{
					num++;
				}
				itemIndex++;
			}
			return num;
		}

		// Token: 0x060027B2 RID: 10162 RVA: 0x000AB106 File Offset: 0x000A9306
		private void Check()
		{
			if (this.UniqueTier1Discovered() >= 10)
			{
				base.Grant();
			}
		}

		// Token: 0x040024FA RID: 9466
		private const int requirement = 10;
	}
}
