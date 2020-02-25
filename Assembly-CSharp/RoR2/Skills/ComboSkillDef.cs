using System;
using EntityStates;

namespace RoR2.Skills
{
	// Token: 0x020004B0 RID: 1200
	public class ComboSkillDef : SkillDef
	{
		// Token: 0x06001D0D RID: 7437 RVA: 0x0007C65B File Offset: 0x0007A85B
		protected SerializableEntityStateType GetNextStateType(GenericSkill skillSlot)
		{
			return HGArrayUtilities.GetSafe<ComboSkillDef.Combo>(this.comboList, ((ComboSkillDef.InstanceData)skillSlot.skillInstanceData).comboCounter).activationStateType;
		}

		// Token: 0x06001D0E RID: 7438 RVA: 0x0007C67D File Offset: 0x0007A87D
		protected override EntityState InstantiateNextState(GenericSkill skillSlot)
		{
			return EntityState.Instantiate(this.GetNextStateType(skillSlot));
		}

		// Token: 0x06001D0F RID: 7439 RVA: 0x0007C68C File Offset: 0x0007A88C
		public override void OnExecute(GenericSkill skillSlot)
		{
			base.OnExecute(skillSlot);
			ComboSkillDef.InstanceData instanceData = (ComboSkillDef.InstanceData)skillSlot.skillInstanceData;
			instanceData.comboCounter++;
			if (instanceData.comboCounter >= this.comboList.Length)
			{
				instanceData.comboCounter = 0;
			}
		}

		// Token: 0x04001A21 RID: 6689
		public ComboSkillDef.Combo[] comboList;

		// Token: 0x020004B1 RID: 1201
		[Serializable]
		public struct Combo
		{
			// Token: 0x04001A22 RID: 6690
			public SerializableEntityStateType activationStateType;
		}

		// Token: 0x020004B2 RID: 1202
		protected class InstanceData : SkillDef.BaseSkillInstanceData
		{
			// Token: 0x04001A23 RID: 6691
			public int comboCounter;
		}
	}
}
