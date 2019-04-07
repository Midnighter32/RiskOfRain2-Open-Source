using System;

namespace RoR2.Achievements
{
	// Token: 0x0200068A RID: 1674
	[RegisterAchievement("CarryLunarItems", "Items.Meteor", null, null)]
	public class CarryLunarItemsAchievement : BaseAchievement
	{
		// Token: 0x0600255B RID: 9563 RVA: 0x000AEF5A File Offset: 0x000AD15A
		public override void OnInstall()
		{
			base.OnInstall();
			this.localUser.onMasterChanged += this.OnMasterChanged;
			this.SetMasterController(this.localUser.cachedMasterController);
		}

		// Token: 0x0600255C RID: 9564 RVA: 0x000AEF8A File Offset: 0x000AD18A
		public override void OnUninstall()
		{
			this.SetMasterController(null);
			this.localUser.onMasterChanged -= this.OnMasterChanged;
			base.OnUninstall();
		}

		// Token: 0x0600255D RID: 9565 RVA: 0x000AEFB0 File Offset: 0x000AD1B0
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

		// Token: 0x0600255E RID: 9566 RVA: 0x000AF030 File Offset: 0x000AD230
		private void OnInventoryChanged()
		{
			if (this.currentInventory)
			{
				int num = 5;
				EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(this.currentInventory.GetEquipmentIndex());
				if (equipmentDef != null && equipmentDef.isLunar)
				{
					num--;
				}
				if (this.currentInventory.HasAtLeastXTotalItemsOfTier(ItemTier.Lunar, num))
				{
					base.Grant();
				}
			}
		}

		// Token: 0x0600255F RID: 9567 RVA: 0x000AF081 File Offset: 0x000AD281
		private void OnMasterChanged()
		{
			this.SetMasterController(this.localUser.cachedMasterController);
		}

		// Token: 0x0400285E RID: 10334
		public const int requirement = 5;

		// Token: 0x0400285F RID: 10335
		private PlayerCharacterMasterController currentMasterController;

		// Token: 0x04002860 RID: 10336
		private Inventory currentInventory;
	}
}
