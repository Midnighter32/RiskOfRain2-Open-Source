using System;

namespace RoR2.Achievements
{
	// Token: 0x020006A2 RID: 1698
	[RegisterAchievement("Discover5Equipment", "Items.EquipmentMagazine", null, null)]
	public class Discover5EquipmentAchievement : BaseAchievement
	{
		// Token: 0x060027B4 RID: 10164 RVA: 0x000AB118 File Offset: 0x000A9318
		public override void OnInstall()
		{
			base.OnInstall();
			base.userProfile.onPickupDiscovered += this.OnPickupDiscovered;
			this.Check();
		}

		// Token: 0x060027B5 RID: 10165 RVA: 0x000AB13D File Offset: 0x000A933D
		public override void OnUninstall()
		{
			base.userProfile.onPickupDiscovered -= this.OnPickupDiscovered;
			base.OnUninstall();
		}

		// Token: 0x060027B6 RID: 10166 RVA: 0x000AB15C File Offset: 0x000A935C
		public override float ProgressForAchievement()
		{
			return (float)this.EquipmentDiscovered() / 5f;
		}

		// Token: 0x060027B7 RID: 10167 RVA: 0x000AB16B File Offset: 0x000A936B
		private void OnPickupDiscovered(PickupIndex pickupIndex)
		{
			if (pickupIndex.equipmentIndex != EquipmentIndex.None)
			{
				this.Check();
			}
		}

		// Token: 0x060027B8 RID: 10168 RVA: 0x000AB180 File Offset: 0x000A9380
		private int EquipmentDiscovered()
		{
			int num = 0;
			EquipmentIndex equipmentIndex = EquipmentIndex.CommandMissile;
			EquipmentIndex equipmentCount = (EquipmentIndex)EquipmentCatalog.equipmentCount;
			while (equipmentIndex < equipmentCount)
			{
				if (base.userProfile.HasDiscoveredPickup(new PickupIndex(equipmentIndex)))
				{
					num++;
				}
				equipmentIndex++;
			}
			return num;
		}

		// Token: 0x060027B9 RID: 10169 RVA: 0x000AB1B9 File Offset: 0x000A93B9
		private void Check()
		{
			if (this.EquipmentDiscovered() >= 5)
			{
				base.Grant();
			}
		}

		// Token: 0x040024FB RID: 9467
		private const int requirement = 5;
	}
}
