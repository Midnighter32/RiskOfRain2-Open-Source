using System;

namespace RoR2.Achievements
{
	// Token: 0x0200069A RID: 1690
	[RegisterAchievement("Discover5Equipment", "Items.EquipmentMagazine", null, null)]
	public class Discover5EquipmentAchievement : BaseAchievement
	{
		// Token: 0x060025A9 RID: 9641 RVA: 0x000AF8D3 File Offset: 0x000ADAD3
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onPickupDiscovered += this.OnPickupDiscovered;
			this.Check();
		}

		// Token: 0x060025AA RID: 9642 RVA: 0x000AF8F8 File Offset: 0x000ADAF8
		public override void OnUninstall()
		{
			this.userProfile.onPickupDiscovered -= this.OnPickupDiscovered;
			base.OnUninstall();
		}

		// Token: 0x060025AB RID: 9643 RVA: 0x000AF917 File Offset: 0x000ADB17
		public override float ProgressForAchievement()
		{
			return (float)this.EquipmentDiscovered() / 5f;
		}

		// Token: 0x060025AC RID: 9644 RVA: 0x000AF926 File Offset: 0x000ADB26
		private void OnPickupDiscovered(PickupIndex pickupIndex)
		{
			if (pickupIndex.equipmentIndex != EquipmentIndex.None)
			{
				this.Check();
			}
		}

		// Token: 0x060025AD RID: 9645 RVA: 0x000AF938 File Offset: 0x000ADB38
		private int EquipmentDiscovered()
		{
			int num = 0;
			for (EquipmentIndex equipmentIndex = EquipmentIndex.CommandMissile; equipmentIndex < EquipmentIndex.Count; equipmentIndex++)
			{
				if (this.userProfile.HasDiscoveredPickup(new PickupIndex(equipmentIndex)))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x060025AE RID: 9646 RVA: 0x000AF96C File Offset: 0x000ADB6C
		private void Check()
		{
			if (this.EquipmentDiscovered() >= 5)
			{
				base.Grant();
			}
		}

		// Token: 0x0400286D RID: 10349
		private const int requirement = 5;
	}
}
