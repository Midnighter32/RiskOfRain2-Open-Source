using System;
using RoR2.Stats;

namespace RoR2.Achievements.Commando
{
	// Token: 0x020006F3 RID: 1779
	[RegisterAchievement("CommandoNonLunarEndurance", "Skills.Commando.ThrowGrenade", null, null)]
	public class CommandoNonLunarEnduranceAchievement : BaseAchievement
	{
		// Token: 0x0600294F RID: 10575 RVA: 0x000ADAE6 File Offset: 0x000ABCE6
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("CommandoBody");
		}

		// Token: 0x06002950 RID: 10576 RVA: 0x000ADB70 File Offset: 0x000ABD70
		private static bool EverPickedUpLunarItems(StatSheet statSheet)
		{
			foreach (ItemIndex key in ItemCatalog.lunarItemList)
			{
				if (statSheet.GetStatValueULong(PerItemStatDef.totalCollected.FindStatDef(key)) > 0UL)
				{
					return true;
				}
			}
			foreach (EquipmentIndex equipmentIndex in EquipmentCatalog.equipmentList)
			{
				if (EquipmentCatalog.GetEquipmentDef(equipmentIndex).isLunar && statSheet.GetStatValueDouble(PerEquipmentStatDef.totalTimeHeld.FindStatDef(equipmentIndex)) > 0.0)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002951 RID: 10577 RVA: 0x000ADC44 File Offset: 0x000ABE44
		private void OnMasterChanged()
		{
			this.SetMasterController(base.localUser.cachedMasterController);
		}

		// Token: 0x06002952 RID: 10578 RVA: 0x000ADC58 File Offset: 0x000ABE58
		private void SetMasterController(PlayerCharacterMasterController playerCharacterMasterController)
		{
			if (base.localUser.cachedMasterController == this.cachedMasterController)
			{
				return;
			}
			bool flag = this.cachedStatsComponent != null;
			this.cachedMasterController = base.localUser.cachedMasterController;
			this.cachedStatsComponent = (this.cachedMasterController ? this.cachedMasterController.GetComponent<PlayerStatsComponent>() : null);
			bool flag2 = this.cachedStatsComponent != null;
			if (flag != flag2)
			{
				if (flag2)
				{
					base.userProfile.onStatsReceived += this.OnStatsChanged;
					return;
				}
				base.userProfile.onStatsReceived -= this.OnStatsChanged;
			}
		}

		// Token: 0x06002953 RID: 10579 RVA: 0x000ADCF4 File Offset: 0x000ABEF4
		private void OnStatsChanged()
		{
			if (this.cachedStatsComponent == null)
			{
				return;
			}
			if (CommandoNonLunarEnduranceAchievement.requirement <= this.cachedStatsComponent.currentStats.GetStatValueULong(StatDef.totalStagesCompleted))
			{
				if (CommandoNonLunarEnduranceAchievement.EverPickedUpLunarItems(this.cachedStatsComponent.currentStats))
				{
					this.SetMasterController(null);
					return;
				}
				base.Grant();
			}
		}

		// Token: 0x06002954 RID: 10580 RVA: 0x000ADD46 File Offset: 0x000ABF46
		protected override void OnBodyRequirementMet()
		{
			base.OnBodyRequirementMet();
			base.localUser.onMasterChanged += this.OnMasterChanged;
			this.OnMasterChanged();
		}

		// Token: 0x06002955 RID: 10581 RVA: 0x000ADD6B File Offset: 0x000ABF6B
		protected override void OnBodyRequirementBroken()
		{
			base.localUser.onMasterChanged -= this.OnMasterChanged;
			this.SetMasterController(null);
			base.OnBodyRequirementBroken();
		}

		// Token: 0x04002550 RID: 9552
		private static readonly ulong requirement = 20UL;

		// Token: 0x04002551 RID: 9553
		private PlayerCharacterMasterController cachedMasterController;

		// Token: 0x04002552 RID: 9554
		private PlayerStatsComponent cachedStatsComponent;
	}
}
