using System;

namespace RoR2.Achievements
{
	// Token: 0x02000690 RID: 1680
	[RegisterAchievement("CarryLunarItems", "Items.Meteor", null, null)]
	public class CarryLunarItemsAchievement : BaseAchievement
	{
		// Token: 0x0600275E RID: 10078 RVA: 0x000AA72B File Offset: 0x000A892B
		public override void OnInstall()
		{
			base.OnInstall();
			base.localUser.onMasterChanged += this.OnMasterChanged;
			this.SetMasterController(base.localUser.cachedMasterController);
		}

		// Token: 0x0600275F RID: 10079 RVA: 0x000AA75B File Offset: 0x000A895B
		public override void OnUninstall()
		{
			this.SetMasterController(null);
			base.localUser.onMasterChanged -= this.OnMasterChanged;
			base.OnUninstall();
		}

		// Token: 0x06002760 RID: 10080 RVA: 0x000AA784 File Offset: 0x000A8984
		private void SetMasterController(PlayerCharacterMasterController newMasterController)
		{
			if (this.currentMasterController == newMasterController)
			{
				return;
			}
			if (this.currentInventory != null)
			{
				this.currentInventory.onInventoryChanged -= this.OnInventoryChanged;
			}
			this.currentMasterController = newMasterController;
			PlayerCharacterMasterController playerCharacterMasterController = this.currentMasterController;
			Inventory inventory;
			if (playerCharacterMasterController == null)
			{
				inventory = null;
			}
			else
			{
				CharacterMaster master = playerCharacterMasterController.master;
				inventory = ((master != null) ? master.inventory : null);
			}
			this.currentInventory = inventory;
			if (this.currentInventory != null)
			{
				this.currentInventory.onInventoryChanged += this.OnInventoryChanged;
			}
		}

		// Token: 0x06002761 RID: 10081 RVA: 0x000AA804 File Offset: 0x000A8A04
		private void OnInventoryChanged()
		{
			if (this.currentInventory)
			{
				int num = 5;
				EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(this.currentInventory.currentEquipmentIndex);
				if (equipmentDef != null && equipmentDef.isLunar)
				{
					num--;
				}
				EquipmentDef equipmentDef2 = EquipmentCatalog.GetEquipmentDef(this.currentInventory.alternateEquipmentIndex);
				if (equipmentDef2 != null && equipmentDef2.isLunar)
				{
					num--;
				}
				if (this.currentInventory.HasAtLeastXTotalItemsOfTier(ItemTier.Lunar, num))
				{
					base.Grant();
				}
			}
		}

		// Token: 0x06002762 RID: 10082 RVA: 0x000AA875 File Offset: 0x000A8A75
		private void OnMasterChanged()
		{
			this.SetMasterController(base.localUser.cachedMasterController);
		}

		// Token: 0x040024EA RID: 9450
		public const int requirement = 5;

		// Token: 0x040024EB RID: 9451
		private PlayerCharacterMasterController currentMasterController;

		// Token: 0x040024EC RID: 9452
		private Inventory currentInventory;
	}
}
