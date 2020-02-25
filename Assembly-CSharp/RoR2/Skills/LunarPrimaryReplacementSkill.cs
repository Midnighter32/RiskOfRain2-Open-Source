using System;

namespace RoR2.Skills
{
	// Token: 0x020004B6 RID: 1206
	public class LunarPrimaryReplacementSkill : SkillDef
	{
		// Token: 0x06001D1A RID: 7450 RVA: 0x0007C78C File Offset: 0x0007A98C
		public override SkillDef.BaseSkillInstanceData OnAssigned(GenericSkill skillSlot)
		{
			LunarPrimaryReplacementSkill.InstanceData instanceData = new LunarPrimaryReplacementSkill.InstanceData();
			instanceData.skillSlot = skillSlot;
			skillSlot.characterBody.onInventoryChanged += instanceData.OnInventoryChanged;
			return instanceData;
		}

		// Token: 0x06001D1B RID: 7451 RVA: 0x0007C7BE File Offset: 0x0007A9BE
		public override void OnUnassigned(GenericSkill skillSlot)
		{
			skillSlot.characterBody.onInventoryChanged -= ((LunarPrimaryReplacementSkill.InstanceData)skillSlot.skillInstanceData).OnInventoryChanged;
		}

		// Token: 0x06001D1C RID: 7452 RVA: 0x0007C7E1 File Offset: 0x0007A9E1
		public override int GetMaxStock(GenericSkill skillSlot)
		{
			return skillSlot.characterBody.inventory.GetItemCount(ItemIndex.LunarPrimaryReplacement) * this.baseMaxStock;
		}

		// Token: 0x06001D1D RID: 7453 RVA: 0x0007C7FC File Offset: 0x0007A9FC
		public override float GetRechargeInterval(GenericSkill skillSlot)
		{
			return (float)skillSlot.characterBody.inventory.GetItemCount(ItemIndex.LunarPrimaryReplacement) * this.baseRechargeInterval;
		}

		// Token: 0x06001D1E RID: 7454 RVA: 0x0007C818 File Offset: 0x0007AA18
		public override int GetRechargeStock(GenericSkill skillSlot)
		{
			return this.GetMaxStock(skillSlot);
		}

		// Token: 0x020004B7 RID: 1207
		private class InstanceData : SkillDef.BaseSkillInstanceData
		{
			// Token: 0x06001D20 RID: 7456 RVA: 0x0007C821 File Offset: 0x0007AA21
			public void OnInventoryChanged()
			{
				this.skillSlot.RecalculateValues();
			}

			// Token: 0x04001A25 RID: 6693
			public GenericSkill skillSlot;
		}
	}
}
