using System;

namespace RoR2.Achievements.Huntress
{
	// Token: 0x020006E9 RID: 1769
	[RegisterAchievement("HuntressCollectCrowbars", "Skills.Huntress.MiniBlink", "CompleteThreeStages", null)]
	public class HuntressCollectCrowbarsAchievement : BaseAchievement
	{
		// Token: 0x06002912 RID: 10514 RVA: 0x000AD590 File Offset: 0x000AB790
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("HuntressBody");
		}

		// Token: 0x06002913 RID: 10515 RVA: 0x000AD59C File Offset: 0x000AB79C
		public override void OnInstall()
		{
			base.OnInstall();
		}

		// Token: 0x06002914 RID: 10516 RVA: 0x000AD5A4 File Offset: 0x000AB7A4
		public override void OnUninstall()
		{
			this.SetCurrentInventory(null);
			base.OnUninstall();
		}

		// Token: 0x06002915 RID: 10517 RVA: 0x000AD5B4 File Offset: 0x000AB7B4
		private void UpdateInventory()
		{
			Inventory inventory = null;
			if (base.localUser.cachedMasterController)
			{
				inventory = base.localUser.cachedMasterController.master.inventory;
			}
			this.SetCurrentInventory(inventory);
		}

		// Token: 0x06002916 RID: 10518 RVA: 0x000AD5F4 File Offset: 0x000AB7F4
		private void SetCurrentInventory(Inventory newInventory)
		{
			if (this.currentInventory == newInventory)
			{
				return;
			}
			if (this.currentInventory != null)
			{
				this.currentInventory.onInventoryChanged -= this.OnInventoryChanged;
			}
			this.currentInventory = newInventory;
			if (this.currentInventory != null)
			{
				this.currentInventory.onInventoryChanged += this.OnInventoryChanged;
				this.OnInventoryChanged();
			}
		}

		// Token: 0x06002917 RID: 10519 RVA: 0x000AD656 File Offset: 0x000AB856
		protected override void OnBodyRequirementMet()
		{
			base.OnBodyRequirementMet();
			base.localUser.onMasterChanged += this.UpdateInventory;
			this.UpdateInventory();
		}

		// Token: 0x06002918 RID: 10520 RVA: 0x000AD67B File Offset: 0x000AB87B
		protected override void OnBodyRequirementBroken()
		{
			base.localUser.onMasterChanged -= this.UpdateInventory;
			this.SetCurrentInventory(null);
			base.OnBodyRequirementBroken();
		}

		// Token: 0x06002919 RID: 10521 RVA: 0x000AD6A1 File Offset: 0x000AB8A1
		private void OnInventoryChanged()
		{
			if (HuntressCollectCrowbarsAchievement.requirement <= this.currentInventory.GetItemCount(ItemIndex.Crowbar))
			{
				base.Grant();
			}
		}

		// Token: 0x04002540 RID: 9536
		private Inventory currentInventory;

		// Token: 0x04002541 RID: 9537
		private static readonly int requirement = 12;
	}
}
